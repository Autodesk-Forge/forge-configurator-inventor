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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared;
using WebApplication.Definitions;
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

        /// <summary>
        /// Adapt the project.
        /// </summary>
        public async Task<FdaStatsDTO> AdoptAsync(ProjectInfo projectInfo, string inputDocUrl)
        {
            _logger.LogInformation($"Adopt project '{projectInfo.Name}'");

            var projectStorage = await _userResolver.GetProjectStorageAsync(projectInfo.Name);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, projectInfo.TopLevelAssembly);

            ProcessingResult result = await _fdaClient.AdoptAsync(adoptionData);
            if (! result.Success)
            {
                var message = $"Failed to process '{projectInfo.Name}' project.";
                _logger.LogError(message);
                throw new FdaProcessingException(message, result.ReportUrl);
            }

            // rearrange generated data according to the parameters hash
            await _arranger.MoveProjectAsync(projectStorage.Project, projectInfo.TopLevelAssembly);

            _logger.LogInformation("Cache the project locally");
            var bucket = await _userResolver.GetBucketAsync();

            // check for adoption errors
            // TECHDEBT: this should be done before `MoveProjectAsync`, but it will left "garbage" at OSS.  Solve it someday.
            var messages = await bucket.DeserializeAsync<Message[]>(projectStorage.Project.OssAttributes.AdoptMessages);
            var errors = messages.Where(m => m.Severity == Severity.Error).Select(m => m.Text).ToArray();
            if (errors.Length > 0) throw new AdoptionException(errors);

            await projectStorage.EnsureLocalAsync(bucket);

            // save adoption statistics
            var ossNames = projectStorage.GetOssNames();
            await bucket.UploadAsJsonAsync(ossNames.StatsAdopt, result.Stats);
            await bucket.CopyAsync(ossNames.StatsAdopt, ossNames.StatsUpdate);
            return FdaStatsDTO.All(result.Stats);
        }

        /// <summary>
        /// Update project state with the parameters (or take it from cache).
        /// </summary>
        public async Task<(ProjectStateDTO dto, FdaStatsDTO stats)> DoSmartUpdateAsync(InventorParameters parameters, string projectId, bool bForceUpdate = false)
        {
            var hash = Crypto.GenerateObjectHashString(parameters);
            _logger.LogInformation($"Incoming parameters hash is {hash}");

            var storage = await _userResolver.GetProjectStorageAsync(projectId);

            FdaStatsDTO stats;
            var localNames = storage.GetLocalNames(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir) && !bForceUpdate)
            {
                _logger.LogInformation("Found cached data.");

                // restore statistics
                var bucket = await _userResolver.GetBucketAsync();
                var statsNative = await bucket.DeserializeAsync<List<Statistics>>(storage.GetOssNames(hash).StatsUpdate);
                stats = FdaStatsDTO.CreditsOnly(statsNative);
            }
            else
            {
                string resultingHash;
                (resultingHash, stats) = await UpdateAsync(storage, parameters, hash, bForceUpdate);
                if (! hash.Equals(resultingHash, StringComparison.Ordinal))
                {
                    _logger.LogInformation($"Update returned different parameters. Hash is {resultingHash}.");
                    await CopyStateAsync(storage.Project, resultingHash, hash, storage.IsAssembly);

                    // update 
                    hash = resultingHash;
                }
            }

            var dto = _dtoGenerator.MakeProjectDTO<ProjectStateDTO>(storage, hash);
            dto.Parameters = Json.DeserializeFile<InventorParameters>(localNames.Parameters);

            return (dto, stats);
        }

        /// <summary>
        /// Generate RFA (or take it from cache).
        /// </summary>
        public async Task<FdaStatsDTO> GenerateRfaAsync(string projectName, string hash)
        {
            _logger.LogInformation($"Generating RFA for hash {hash}");

            ProjectStorage storage = await _userResolver.GetProjectStorageAsync(projectName);
            Project project = storage.Project;

            //// *********************************************
            //// temporary fail *********************************************
            //_logger.LogError($"Failed to generate SAT file");
            //throw new FdaProcessingException($"Failed to generate SAT file", "https://localhost:5000/#");
            //// *********************************************


            var ossNames = project.OssNameProvider(hash);

            var bucket = await _userResolver.GetBucketAsync();
            // check if RFA file is already generated
            if (await bucket.ObjectExistsAsync(ossNames.Rfa))
            {
                var stats = await bucket.DeserializeAsync<Statistics[]>(ossNames.StatsRFA);
                return FdaStatsDTO.CreditsOnly(stats);
            }

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNames.GetCurrentModel(storage.IsAssembly));
            ProcessingArgs satData = await _arranger.ForSatAsync(inputDocUrl, storage.Metadata.TLA);
            ProcessingArgs rfaData = await _arranger.ForRfaAsync(satData.SatUrl);

            ProcessingResult result = await _fdaClient.GenerateRfa(satData, rfaData);
            if (!result.Success)
            {
                _logger.LogError($"{result.ErrorMessage} for project {project.Name} and hash {hash}");
                throw new FdaProcessingException($"{result.ErrorMessage} for project {project.Name} and hash {hash}", result.ReportUrl);
            }

            await _arranger.MoveRfaAsync(project, hash);
            await bucket.UploadAsJsonAsync(ossNames.StatsRFA, result.Stats);
            return FdaStatsDTO.All(result.Stats);
        }

        public async Task<(FdaStatsDTO, int)> ExportDrawingPdfAsync(string projectName, string hash, string drawingKey)
        {
            _logger.LogInformation($"Getting drawing pdf for hash {hash}");

            ProjectStorage storage = await _userResolver.GetProjectStorageAsync(projectName);
            Project project = storage.Project;

            var ossNames = project.OssNameProvider(hash);
            var ossAttributes = project.OssAttributes;

            // get drawing index from drawing specified
            var bucket = await _userResolver.GetBucketAsync();

            // ????? cache drawingsList instead of reading it every time ?????
            var drawings = await bucket.DeserializeAsync<List<string>>(ossAttributes.DrawingsList);
            int index = drawings.ToList().IndexOf(drawingKey);
            var drawingIdx = index >= 0 ? index : 0;

            // check if Drawing viewables file is already generated
            try
            {
                bool generated = false;

                ApiResponse<dynamic> ossObjectResponse = await bucket.GetObjectAsync(ossNames.DrawingPdf(drawingIdx));
                if (ossObjectResponse != null)
                {
                    await using Stream objectStream = ossObjectResponse.Data;

                    // zero length means that there is nothing to generate, but processed and do not continue
                    generated = objectStream.Length > 0;
                }

                if (generated)
                {
                    var nativeStats = await bucket.DeserializeAsync<List<Statistics>>(ossNames.StatsDrawingPDF(drawingIdx));
                    return (FdaStatsDTO.CreditsOnly(nativeStats), drawingIdx);
                }
                else
                {
                    return (null, drawingIdx);
                }
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                // the file does not exist, so just swallow
            }
            _logger.LogInformation($"Drawing PDF for hash {hash}: generating");

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNames.GetCurrentModel(storage.IsAssembly));
            ProcessingArgs drawingData = await _arranger.ForDrawingPdfAsync(inputDocUrl, drawingKey, storage.Metadata.TLA);

            ProcessingResult result = await _fdaClient.ExportDrawingAsync(drawingData);
            if (!result.Success)
            {
                _logger.LogError($"{result.ErrorMessage} for project {project.Name} and hash {hash}");
                throw new FdaProcessingException($"{result.ErrorMessage} for project {project.Name} and hash {hash}", result.ReportUrl);
            }

            // move to the right place
            await _arranger.MoveDrawingPdfAsync(project, drawingIdx, hash);

            // check if Drawing PDF file is generated
            try
            {
                await bucket.CreateSignedUrlAsync(ossNames.DrawingPdf(drawingIdx));

                // handle statistics
                await bucket.UploadAsJsonAsync(ossNames.StatsDrawingPDF(drawingIdx), result.Stats);
                _logger.LogInformation($"Drawing PDF for hash {hash} is generated");
                return (FdaStatsDTO.All(result.Stats), drawingIdx);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                // the file does not exist after generating drawing, so just mark with zero length that we already processed it
                await bucket.UploadObjectAsync(ossNames.DrawingPdf(drawingIdx), new MemoryStream(0));

                _logger.LogError($"Drawing PDF for hash {hash} is NOT generated");
                return (null, drawingIdx);
            }
        }

        /// <summary>
        /// Generate Drawing zip with folder structure (or take it from cache).
        /// </summary>
        public async Task<FdaStatsDTO> GenerateDrawingAsync(string projectName, string hash)
        {
            _logger.LogInformation($"Generating Drawing for hash {hash}");

            ProjectStorage storage = await _userResolver.GetProjectStorageAsync(projectName);
            Project project = storage.Project;

            var ossNames = project.OssNameProvider(hash);

            var bucket = await _userResolver.GetBucketAsync();
            // check if Drawing file is already generated
            if (await bucket.ObjectExistsAsync(ossNames.Drawing))
            {
                var stats = await bucket.DeserializeAsync<Statistics[]>(ossNames.StatsDrawings);
                return FdaStatsDTO.CreditsOnly(stats);
            }

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNames.GetCurrentModel(storage.IsAssembly));

            ProcessingArgs drawingData = await _arranger.ForDrawingAsync(inputDocUrl, storage.Metadata.TLA);
            ProcessingResult result = await _fdaClient.GenerateDrawing(drawingData);
            if (!result.Success)
            {
                _logger.LogError($"{result.ErrorMessage} for project {project.Name} and hash {hash}");
                throw new FdaProcessingException($"{result.ErrorMessage} for project {project.Name} and hash {hash}", result.ReportUrl);
            }

            await _arranger.MoveDrawingAsync(project, hash);

            await bucket.UploadAsJsonAsync(ossNames.StatsDrawings, result.Stats);
            return FdaStatsDTO.All(result.Stats);
        }

        public async Task FileTransferAsync(string source, string target)
        {
            ProcessingResult result = await _fdaClient.TransferAsync(source, target);
            if (!result.Success) throw new ApplicationException($"Failed to transfer project file {source}");

            _logger.LogInformation("File transferred.");
        }

        /// <summary>
        /// Generate project data for the given parameters and cache results locally.
        /// </summary>
        /// <returns>Resulting parameters hash</returns>
        private async Task<(string hash, FdaStatsDTO stats)> UpdateAsync(ProjectStorage storage, InventorParameters parameters, string hash, bool bForceUpdate = false)
        {
            _logger.LogInformation("Update the project");
            var bucket = await _userResolver.GetBucketAsync();

            var isUpdateExists = bForceUpdate ? false : await IsGenerated(bucket, storage.GetOssNames(hash));
            FdaStatsDTO stats;
            if (isUpdateExists)
            {
                _logger.LogInformation("Detected existing outputs at OSS");
                var statsNative = await bucket.DeserializeAsync<List<Statistics>>(storage.GetOssNames(hash).StatsUpdate);
                stats = FdaStatsDTO.CreditsOnly(statsNative);
            }
            else
            {
                Project project = storage.Project;

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

                // process statistics
                await bucket.UploadAsJsonAsync(storage.GetOssNames(hash).StatsUpdate, result.Stats);
                stats = FdaStatsDTO.All(result.Stats);
            }

            _logger.LogInformation($"Cache the project locally ({hash})");

            // and now cache the generated stuff locally
            await storage.EnsureViewablesAsync(bucket, hash);

            return (hash, stats);
        }

        /// <summary>
        /// Checks if project has outputs for the given parameters hash.
        /// NOTE: it checks presence of `parameters.json` only.
        /// </summary>
        private static async Task<bool> IsGenerated(OssBucket bucket, OSSObjectNameProvider ossNames)
        {
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
                bucket.CopyAsync(ossFrom.Bom, ossTo.Bom),
                bucket.CopyAsync(ossFrom.StatsUpdate, ossTo.StatsUpdate));

            _logger.LogInformation($"Cache the project locally ({hashTo})");
        }
    }
}
