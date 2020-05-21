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
        public async Task AdoptAsync(ProjectInfo projectInfo)
        {
            _logger.LogInformation("Adopt the project");

            var project = _resourceProvider.GetProject(projectInfo.Name);

            var inputDocUrl = await _forgeOSS.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.OSSSourceModel);
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
            var hash = Crypto.GenerateObjectHashString(parameters);
            //_logger.LogInformation(JsonSerializer.Serialize(parameters));
            _logger.LogInformation($"Parameters hash is {hash}");

            var project = _resourceProvider.GetProject(projectInfo.Name);
            var localNames = project.LocalNameProvider(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir))
            {
                _logger.LogInformation("Found cached data.");
            }
            else
            {
                await UpdateAsync(project, projectInfo.TopLevelAssembly, parameters);
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
        private async Task UpdateAsync(Project project, string tlaFilename, InventorParameters parameters)
        {
            _logger.LogInformation("Update the project");

            var inputDocUrl = await _forgeOSS.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.OSSSourceModel);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, tlaFilename, parameters);

            bool success = await _fdaClient.AdoptAsync(adoptionData);
            if (! success) throw new ApplicationException($"Failed to update {project.Name}");

            // rearrange generated data according to the parameters hash
            string hash = await _arranger.MoveViewablesAsync(project);

            _logger.LogInformation("Cache the project locally");

            // and now cache the generate stuff locally
            var projectStorage = new ProjectStorage(project, _resourceProvider);
            await projectStorage.EnsureViewablesAsync(_httpClientFactory.CreateClient(), _forgeOSS, hash);
        }
    }
}
