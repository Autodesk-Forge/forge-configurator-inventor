﻿using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        public async Task AdoptAsync(ProjectInfo projectInfo, string inputDocUrl)
        {
            _logger.LogInformation("Adopt the project");

            var project = await _userResolver.GetProjectAsync(projectInfo.Name);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, projectInfo.TopLevelAssembly);

            ProcessingResult result = await _fdaClient.AdoptAsync(adoptionData);
            if (! result.Success)
            {
                _logger.LogError($"Failed to process '{project.Name}' project.");
                throw new FdaProcessingException($"Failed to process '{project.Name}' project.", result.ReportUrl);
            }

            // rearrange generated data according to the parameters hash
            await _arranger.MoveProjectAsync(project, projectInfo.TopLevelAssembly);

            _logger.LogInformation("Cache the project locally");

            var bucket = await _userResolver.GetBucketAsync();

            // and now cache the generated stuff locally
            var projectLocalStorage = new ProjectStorage(project);
            await projectLocalStorage.EnsureLocalAsync(bucket);
        }

        /// <summary>
        /// Update project state with the parameters (or take it from cache).
        /// </summary>
        public async Task<ProjectStateDTO> DoSmartUpdateAsync(InventorParameters parameters, string projectId)
        {
            var hash = Crypto.GenerateObjectHashString(parameters);
            //_logger.LogInformation(JsonSerializer.Serialize(parameters));
            _logger.LogInformation($"Incoming parameters hash is {hash}");

            var storage = await _userResolver.GetProjectStorageAsync(projectId);
            var project = storage.Project;

            var localNames = project.LocalNameProvider(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir))
            {
                _logger.LogInformation("Found cached data.");
            }
            else
            {
                var resultingHash = await UpdateAsync(project, storage.Metadata.TLA, parameters);
                if (! hash.Equals(resultingHash, StringComparison.Ordinal))
                {
                    _logger.LogInformation($"Update returned different parameters. Hash is {resultingHash}.");
                    await CopyStateAsync(project, resultingHash, hash);

                    // update 
                    hash = resultingHash;
                }
            }

            var dto = _dtoGenerator.MakeProjectDTO<ProjectStateDTO>(project, hash);
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

            var ossNameProvider = project.OssNameProvider(hash);

            var bucket = await _userResolver.GetBucketAsync();
            // check if RFA file is already generated
            try
            {
                // TODO: this might be ineffective as some "get details" API call
                await bucket.CreateSignedUrlAsync(ossNameProvider.Rfa);
                return;
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                // the file does not exist, so just swallow
            }

            // OK, nothing in cache - generate it now
            var inputDocUrl = await bucket.CreateSignedUrlAsync(ossNameProvider.CurrentModel);
            ProcessingArgs satData = await _arranger.ForSatAsync(inputDocUrl, storage.Metadata.TLA);
            ProcessingArgs rfaData = await _arranger.ForRfaAsync(satData.SatUrl);

            ProcessingResult result = await _fdaClient.GenerateRfa(satData, rfaData);
            if (! result.Success) throw new ApplicationException($"Failed to generate rfa for project {project.Name} and hash {hash}");

            await _arranger.MoveRfaAsync(project, hash);
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
        private async Task<string> UpdateAsync(Project project, string tlaFilename, InventorParameters parameters)
        {
            _logger.LogInformation("Update the project");
            var bucket = await _userResolver.GetBucketAsync();

            var inputDocUrl = await bucket.CreateSignedUrlAsync(project.OSSSourceModel);
            UpdateData updateData = await _arranger.ForUpdateAsync(inputDocUrl, tlaFilename, parameters);

            ProcessingResult result = await _fdaClient.UpdateAsync(updateData);
            if (!result.Success) 
            {
                 _logger.LogError($"Failed to update '{project.Name}' project.");
                throw new FdaProcessingException($"Failed to update '{project.Name}' project.", result.ReportUrl);
            }

            _logger.LogInformation("Moving files around");

            // rearrange generated data according to the parameters hash
            string hash = await _arranger.MoveViewablesAsync(project);

            _logger.LogInformation($"Cache the project locally ({hash})");

            // and now cache the generate stuff locally
            var projectStorage = new ProjectStorage(project);
            await projectStorage.EnsureViewablesAsync(bucket, hash);

            return hash;
        }

        private async Task CopyStateAsync(Project project, string hashFrom, string hashTo)
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
                bucket.CopyAsync(ossFrom.CurrentModel, ossTo.CurrentModel),
                bucket.CopyAsync(ossFrom.ModelView, ossTo.ModelView));

            _logger.LogInformation($"Cache the project locally ({hashTo})");
        }
    }
}
