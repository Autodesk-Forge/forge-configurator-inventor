using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Business logic for project tasks (adapt, update parameters)
    /// </summary>
    public class ProjectWork
    {
        private readonly ILogger<ProjectWork> _logger;
        private readonly ResourceProvider _resourceProvider;
        private readonly Arranger _arranger;
        private readonly FdaClient _fdaClient;
        private readonly IForgeOSS _forgeOSS;
        private readonly DtoGenerator _dtoGenerator;

        public ProjectWork(ILogger<ProjectWork> logger, ResourceProvider resourceProvider, Arranger arranger, FdaClient fdaClient,
                            IForgeOSS forgeOSS, DtoGenerator dtoGenerator)
        {
            _logger = logger;
            _resourceProvider = resourceProvider;
            _arranger = arranger;
            _fdaClient = fdaClient;
            _forgeOSS = forgeOSS;
            _dtoGenerator = dtoGenerator;
        }

        /// <summary>
        /// Adapt the project.
        /// </summary>
        public async Task AdoptAsync(ProjectInfo projectInfo, string inputDocUrl)
        {
            _logger.LogInformation("Adopt the project");

            var project = _resourceProvider.GetProject(projectInfo.Name);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, projectInfo.TopLevelAssembly);

            bool success = await _fdaClient.AdoptAsync(adoptionData);
            if (! success)
            {
                _logger.LogError($"Failed to process '{project.Name}' project.");
                // TODO: should we fail hard?
            }
            else
            {
                // rearrange generated data according to the parameters hash
                await _arranger.MoveProjectAsync(project, projectInfo.TopLevelAssembly);

                _logger.LogInformation("Cache the project locally");

                // and now cache the generate stuff locally
                var projectLocalStorage = new ProjectStorage(project, _resourceProvider);
                await projectLocalStorage.EnsureLocalAsync(_forgeOSS);
            }
        }

        /// <summary>
        /// Update project state with the parameters (or take it from cache).
        /// </summary>
        public async Task<ProjectStateDTO> DoSmartUpdateAsync(ProjectInfo projectInfo, InventorParameters parameters)
        {
            var hash = Crypto.GenerateObjectHashString(parameters);
            //_logger.LogInformation(JsonSerializer.Serialize(parameters));
            _logger.LogInformation($"Incoming parameters hash is {hash}");

            var project = _resourceProvider.GetProject(projectInfo.Name);
            var localNames = project.LocalNameProvider(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir))
            {
                _logger.LogInformation("Found cached data.");
            }
            else
            {
                var resultingHash = await UpdateAsync(project, projectInfo.TopLevelAssembly, parameters);
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

        public async Task FileTransferAsync(string source, string target)
        {
            bool success = await _fdaClient.TransferAsync(source, target);
            if (!success) throw new ApplicationException($"Failed to transfer project file {source}");

            _logger.LogInformation("File transferred.");
        }

        /// <summary>
        /// Generate project data for the given parameters and cache results locally.
        /// </summary>
        private async Task<string> UpdateAsync(Project project, string tlaFilename, InventorParameters parameters)
        {
            _logger.LogInformation("Update the project");

            var inputDocUrl = await _forgeOSS.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.OSSSourceModel);
            UpdateData updateData = await _arranger.ForUpdateAsync(inputDocUrl, tlaFilename, parameters);

            bool success = await _fdaClient.UpdateAsync(updateData);
            if (! success) throw new ApplicationException($"Failed to update {project.Name}");

            _logger.LogInformation("Moving files around");

            // rearrange generated data according to the parameters hash
            string hash = await _arranger.MoveViewablesAsync(project);

            _logger.LogInformation($"Cache the project locally ({hash})");

            // and now cache the generate stuff locally
            var projectStorage = new ProjectStorage(project, _resourceProvider);
            await projectStorage.EnsureViewablesAsync(_forgeOSS, hash);

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


            // copy OSS files
            OSSObjectNameProvider ossFrom = project.OssNameProvider(hashFrom);
            OSSObjectNameProvider ossTo = project.OssNameProvider(hashTo);

            await Task.WhenAll(
                _forgeOSS.CopyAsync(_resourceProvider.BucketKey, ossFrom.Parameters, ossTo.Parameters),
                _forgeOSS.CopyAsync(_resourceProvider.BucketKey, ossFrom.ModelView, ossTo.ModelView));

            _logger.LogInformation($"Cache the project locally ({hashTo})");
        }
    }
}
