/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Shared;
using WebApplication.Definitions;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Business logic for project tasks (adapt, update parameters, rfa generation)
    /// </summary>
    public class ProjectWork
    {
        private readonly ILogger<ProjectWork> _logger;
        private readonly Arranger _arranger;
        private readonly FdaClient _fdaClient;
        private readonly DtoGenerator _dtoGenerator;
        private readonly UserResolver _userResolver;

        public ProjectWork(ILogger<ProjectWork> logger, Arranger arranger, FdaClient fdaClient,
                            DtoGenerator dtoGenerator, UserResolver userResolver)
        {
            _logger = logger;
            _arranger = arranger;
            _fdaClient = fdaClient;
            _dtoGenerator = dtoGenerator;
            _userResolver = userResolver;
        }

        public async Task<string> SplitAsync(ProjectInfo projectInfo, string inputDocUrl)
        {
            _logger.LogInformation("Split drawings");

            var projectStorage = await _userResolver.GetProjectStorageAsync(projectInfo.Name);
            var splitData = await _arranger.ForSplitAsync(inputDocUrl, projectInfo.TopLevelAssembly);

            ProcessingResult result = await _fdaClient.SplitAsync(splitData);
            if (!result.Success)
            {
                _logger.LogError($"Failed to split '{projectInfo.Name}' project.");
                throw new FdaProcessingException($"Failed to splt '{projectInfo.Name}' project.", result.ReportUrl);
            }

            var url = await _arranger.MoveAfterSplitAsync(projectStorage);
            return url;
        }

        internal async Task<string> GetUploadToAsync(bool isAssembly, string projectName)
        {
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName);
            return isAssembly ? _arranger.UploadedModel : projectStorage.Project.OSSSourceModel;
        }

        /// <summary>
        /// Adapt the project.
        /// </summary>
        public async Task AdoptAsync(ProjectInfo projectInfo, string inputDocUrl)
        {
            _logger.LogInformation("Adopt the project");

            var projectStorage = await _userResolver.GetProjectStorageAsync(projectInfo.Name);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, projectInfo.TopLevelAssembly);

            ProcessingResult result = await _fdaClient.AdoptAsync(adoptionData);
            if (! result.Success)
            {
                _logger.LogError($"Failed to process '{projectInfo.Name}' project.");
                throw new FdaProcessingException($"Failed to process '{projectInfo.Name}' project.", result.ReportUrl);
            }

            // rearrange generated data according to the parameters hash
            await _arranger.MoveProjectAsync(projectStorage.Project, projectInfo.TopLevelAssembly);

            _logger.LogInformation("Cache the project locally");
            var bucket = await _userResolver.GetBucketAsync();

            await projectStorage.EnsureLocalAsync(bucket);
        }

        /// <summary>
        /// Update project state with the parameters (or take it from cache).
        /// </summary>
        public async Task<ProjectStateDTO> DoSmartUpdateAsync(InventorParameters parameters, string projectId, bool bForceUpdate = false)
        {
            var hash = Crypto.GenerateObjectHashString(parameters);
            //_logger.LogInformation(JsonSerializer.Serialize(parameters));
            _logger.LogInformation($"Incoming parameters hash is {hash}");

            var storage = await _userResolver.GetProjectStorageAsync(projectId);
            var project = storage.Project;

            var localNames = project.LocalNameProvider(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir) && !bForceUpdate)
            {
                _logger.LogInformation("Found cached data.");
            }
            else
            {
                var resultingHash = await UpdateAsync(project, storage, parameters, hash, bForceUpdate);
                if (! hash.Equals(resultingHash, StringComparison.Ordinal))
                {
                    _logger.LogInformation($"Update returned different parameters. Hash is {resultingHash}.");
                    await CopyStateAsync(project, resultingHash, hash, storage.IsAssembly);

                    // update 
                    hash = resultingHash;
                }
            }

            var dto = _dtoGenerator.MakeProjectDTO<ProjectStateDTO>(project, hash, storage.IsAssembly, storage.Metadata.HasDrawings);
            dto.Parameters = Json.DeserializeFile<InventorParameters>(localNames.Parameters);

            return dto;
        }


        /// <summary>
        /// Generate RFA (or take it from cache).
        /// </summary>
        public async Task GenerateRfaAsync(string projectName, string hash)
        {
            _logger.LogInformation($"Generating RFA for hash {hash}");

            ProjectStorage storage = await _userResolver.GetProjectStorageAsync(projectName);
            Project project = storage.Project;

            //// *********************************************
            //// temporary fail *********************************************
            //_logger.LogError($"Failed to generate SAT file");
            //throw new FdaProcessingException($"Failed to generate SAT file", "https://localhost:5000/#");
            //// *********************************************


            var ossNameProvider = project.OssNameProvider(hash);

            var bucket = await _userResolver.GetBucketAsync();
            // check if RFA file is already generated
            var rfaExists = await bucket.TryToCreateSignedUrlForReadAsync(ossNameProvider.Rfa) != null;
            if (rfaExists)
                return;

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNameProvider.GetCurrentModel(storage.IsAssembly));
            ProcessingArgs satData = await _arranger.ForSatAsync(inputDocUrl, storage.Metadata.TLA);
            ProcessingArgs rfaData = await _arranger.ForRfaAsync(satData.SatUrl);

            ProcessingResult result = await _fdaClient.GenerateRfa(satData, rfaData);
            if (!result.Success)
            {
                _logger.LogError($"{result.ErrorMessage} for project {project.Name} and hash {hash}");
                throw new FdaProcessingException($"{result.ErrorMessage} for project {project.Name} and hash {hash}", result.ReportUrl);
            }

            await _arranger.MoveRfaAsync(project, hash);
        }

        public async Task<bool> ExportDrawingViewablesAsync(string projectName, string hash)
        {
            _logger.LogInformation($"Generating drawing viewables for hash {hash}");

            ProjectStorage storage = await _userResolver.GetProjectStorageAsync(projectName);

            var bucket = await _userResolver.GetBucketAsync();

            // check if drawing source exists, if not, nothing to do
            var sourceDrawings = await bucket.TryToCreateSignedUrlForReadAsync(storage.Project.OSSSourceDrawings);
            if (string.IsNullOrEmpty(sourceDrawings))
                return false; // nothing generated (no drawings exists)

            Project project = storage.Project;

            var ossNameProvider = project.OssNameProvider(hash);

            var drawingViewables = await bucket.TryToCreateSignedUrlForReadAsync(ossNameProvider.DrawingViewables);
            bool generated = !string.IsNullOrEmpty(drawingViewables);

            if (generated)
                return true;

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNameProvider.GetCurrentModel(storage.IsAssembly));
            string inputDrawingUrl = await bucket.TryToCreateSignedUrlForReadAsync(project.OSSSourceDrawings);

            ProcessingArgs drawingData = await _arranger.ForDrawingViewablesAsync(inputDocUrl, inputDrawingUrl, storage.Metadata.TLA);

            ProcessingResult result = await _fdaClient.ExportDrawingAsync(drawingData);
            if (!result.Success)
            {
                _logger.LogError($"{result.ErrorMessage} for project {project.Name} and hash {hash}");
                throw new FdaProcessingException($"{result.ErrorMessage} for project {project.Name} and hash {hash}", result.ReportUrl);
            }

            await _arranger.MoveDrawingViewablesAsync(project, hash);

            // check if Drawing viewables file is generated
            var drawingViewablesUrl = await bucket.TryToCreateSignedUrlForReadAsync(ossNameProvider.DrawingViewables);
            // return if generated
            return !string.IsNullOrEmpty(drawingViewablesUrl);
        }

        /// <summary>
        /// Generate Drawing zip with folder structure (or take it from cache).
        /// </summary>
        public async Task GenerateDrawingAsync(string projectName, string hash)
        {
            _logger.LogInformation($"Generating Drawing for hash {hash}");

            ProjectStorage storage = await _userResolver.GetProjectStorageAsync(projectName);
            Project project = storage.Project;

            //// *********************************************
            //// temporary fail *********************************************
            //_logger.LogError($"Failed to generate SAT file");
            //throw new FdaProcessingException($"Failed to generate SAT file", "https://localhost:5000/#");
            //// *********************************************


            var ossNameProvider = project.OssNameProvider(hash);

            var bucket = await _userResolver.GetBucketAsync();
            // check if Drawing file is already generated
            var drawingUrl = await bucket.TryToCreateSignedUrlForReadAsync(ossNameProvider.Drawing);
            if (drawingUrl != null)
                return;

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNameProvider.GetCurrentModel(storage.IsAssembly));
            string inputDrawingUrl = await bucket.TryToCreateSignedUrlForReadAsync(project.OSSSourceDrawings);

            ProcessingArgs drawingData = await _arranger.ForDrawingAsync(inputDocUrl, inputDrawingUrl, storage.Metadata.TLA);
            ProcessingResult result = await _fdaClient.GenerateDrawing(drawingData);
            if (!result.Success)
            {
                _logger.LogError($"{result.ErrorMessage} for project {project.Name} and hash {hash}");
                throw new FdaProcessingException($"{result.ErrorMessage} for project {project.Name} and hash {hash}", result.ReportUrl);
            }

            await _arranger.MoveDrawingAsync(project, hash);
        }

        public async Task<string> FileTransferAsync(string source, RetryPolicy policy, OssBucket bucket, string target)
        {
            string uploadTo = string.IsNullOrEmpty(target) ? _arranger.UploadedModel : target;
            string signedUrl = await policy.ExecuteAsync(async () => await bucket.CreateSignedUrlAsync(uploadTo, ObjectAccess.ReadWrite));

            ProcessingResult result = await _fdaClient.TransferAsync(source, signedUrl);
            if (!result.Success) throw new ApplicationException($"Failed to transfer project file {source}");

            _logger.LogInformation("File transferred.");

            return signedUrl;
        }

        /// <summary>
        /// Generate project data for the given parameters and cache results locally.
        /// </summary>
        /// <returns>Resulting parameters hash</returns>
        private async Task<string> UpdateAsync(Project project, ProjectStorage storage, InventorParameters parameters, string hash, bool bForceUpdate = false)
        {
            _logger.LogInformation("Update the project");
            var bucket = await _userResolver.GetBucketAsync();

            var isUpdateExists = bForceUpdate ? false : await IsGenerated(project, bucket, hash);
            if (isUpdateExists)
            {
                _logger.LogInformation("Detected existing outputs at OSS");
            }
            else
            {
                var inputDocUrl = await bucket.CreateSignedUrlAsync(project.OSSSourceModel);
                UpdateData updateData = await _arranger.ForUpdateAsync(inputDocUrl, storage.Metadata.TLA, parameters);

                ProcessingResult result = await _fdaClient.UpdateAsync(updateData);
                if (!result.Success)
                {
                    _logger.LogError($"Failed to update '{project.Name}' project.");
                    throw new FdaProcessingException($"Failed to update '{project.Name}' project.", result.ReportUrl);
                }

                _logger.LogInformation("Moving files around");

                // rearrange generated data according to the parameters hash
                // NOTE: hash might be changed if Inventor adjust them!
                hash = await _arranger.MoveViewablesAsync(project, storage.IsAssembly);
            }

            _logger.LogInformation($"Cache the project locally ({hash})");

            // and now cache the generate stuff locally
            var projectStorage = new ProjectStorage(project);
            await projectStorage.EnsureViewablesAsync(bucket, hash);

            return hash;
        }

        /// <summary>
        /// Checks if project has outputs for the given parameters hash.
        /// NOTE: it checks presence of `parameters.json` only.
        /// </summary>
        private static async Task<bool> IsGenerated(Project project, OssBucket bucket, string hash)
        {
            var ossNames = project.OssNameProvider(hash);
            return await bucket.ObjectExistsAsync(ossNames.Parameters);
        }

        private async Task CopyStateAsync(Project project, string hashFrom, string hashTo, bool isAssembly)
        {
            // see if the dir exists already
            LocalNameProvider localTo = project.LocalNameProvider(hashTo);
            if (Directory.Exists(localTo.BaseDir))
            {
                _logger.LogInformation($"Found existing {hashTo}");
            }

            // copy local file structure
            LocalNameProvider localFrom = project.LocalNameProvider(hashFrom);

            // SOMEDAY: performance improvement - replace with symlink when it's supported
            // by netcore (https://github.com/dotnet/runtime/issues/24271)
            FileSystem.CopyDir(localFrom.BaseDir, localTo.BaseDir);

            var bucket = await _userResolver.GetBucketAsync();

            // copy OSS files
            OSSObjectNameProvider ossFrom = project.OssNameProvider(hashFrom);
            OSSObjectNameProvider ossTo = project.OssNameProvider(hashTo);

            await Task.WhenAll(
                bucket.CopyAsync(ossFrom.Parameters, ossTo.Parameters),
                bucket.CopyAsync(ossFrom.GetCurrentModel(isAssembly), ossTo.GetCurrentModel(isAssembly)),
                bucket.CopyAsync(ossFrom.ModelView, ossTo.ModelView),
                bucket.CopyAsync(ossFrom.Bom, ossTo.Bom));

            _logger.LogInformation($"Cache the project locally ({hashTo})");
        }
    }
}
