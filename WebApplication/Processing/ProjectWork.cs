using System;
using System.IO;
using System.Net.Http;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Arranger _arranger;
        private readonly FdaClient _fdaClient;
        private readonly IForgeOSS _forgeOSS;

        public ProjectWork(ILogger<ProjectWork> logger, ResourceProvider resourceProvider, IHttpClientFactory httpClientFactory, Arranger arranger, FdaClient fdaClient, IForgeOSS forgeOSS)
        {
            _logger = logger;
            _resourceProvider = resourceProvider;
            _httpClientFactory = httpClientFactory;
            _arranger = arranger;
            _fdaClient = fdaClient;
            _forgeOSS = forgeOSS;
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
            }
            else
            {
                // rearrange generated data according to the parameters hash
                await _arranger.MoveProjectAsync(project, projectInfo.TopLevelAssembly);

                _logger.LogInformation("Cache the project locally");

                // and now cache the generate stuff locally
                var projectLocalStorage = new ProjectStorage(project, _resourceProvider);
                await projectLocalStorage.EnsureLocalAsync(_httpClientFactory.CreateClient(), _forgeOSS);
            }
        }

        /// <summary>
        /// Update project state with the parameters (or take it from cache).
        /// </summary>
        public async Task<ProjectStateDTO> DoSmartUpdateAsync(ProjectInfo projectInfo, InventorParameters parameters)
        {
            var incomingHash = Crypto.GenerateObjectHashString(parameters);
            //_logger.LogInformation(JsonSerializer.Serialize(parameters));
            _logger.LogInformation($"Parameters hash is {incomingHash}");

            var project = _resourceProvider.GetProject(projectInfo.Name);
            var localNames = project.LocalNameProvider(incomingHash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir))
            {
                _logger.LogInformation("Found cached data.");
            }
            else
            {
                var resultingHash = await UpdateAsync(project, projectInfo.TopLevelAssembly, parameters);
                if (! incomingHash.Equals(resultingHash, StringComparison.Ordinal))
                {
                    _logger.LogInformation($"Update returned different parameters. Hash is {resultingHash}.");
                    await CopyStateAsync(project, resultingHash, incomingHash);
                }
            }

            return new ProjectStateDTO
                    {
                        Svf = _resourceProvider.ToDataUrl(localNames.SvfDir),
                        Parameters = Json.DeserializeFile<InventorParameters>(localNames.Parameters)
                    };
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
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, tlaFilename, parameters);

            bool success = await _fdaClient.AdoptAsync(adoptionData);
            if (! success) throw new ApplicationException($"Failed to update {project.Name}");

            // rearrange generated data according to the parameters hash
            string hash = await _arranger.MoveViewablesAsync(project);

            _logger.LogInformation($"Cache the project locally ({hash})");

            // and now cache the generate stuff locally
            var projectStorage = new ProjectStorage(project, _resourceProvider);
            await projectStorage.EnsureViewablesAsync(_httpClientFactory.CreateClient(), _forgeOSS, hash);

            return hash;
        }

        private async Task CopyStateAsync(Project project, string hashFrom, string hashTo)
        {
            LocalNameProvider localTo = project.LocalNameProvider(hashTo);
            if (Directory.Exists(localTo.BaseDir))
            {
                _logger.LogInformation($"Found existing {hashTo}");
            }

            // copy local file structure
            LocalNameProvider localFrom = project.LocalNameProvider(hashFrom);
            FileSystem.CopyDir(localFrom.BaseDir, localTo.BaseDir); // TODO: performance improvement - replace with symlink


            // copy OSS files
            OSSObjectNameProvider ossFrom = project.OssNameProvider(hashFrom);
            OSSObjectNameProvider ossTo = project.OssNameProvider(hashTo);

            await _forgeOSS.CopyAsync(_resourceProvider.BucketKey, ossFrom.Parameters, ossTo.Parameters);
            await _forgeOSS.CopyAsync(_resourceProvider.BucketKey, ossFrom.ModelView, ossTo.ModelView);

            _logger.LogInformation($"Cache the project locally ({hashTo})");
        }
    }
}
