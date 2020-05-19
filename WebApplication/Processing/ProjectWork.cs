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

        public ProjectWork(ILogger<ProjectWork> logger, ResourceProvider resourceProvider, IHttpClientFactory httpClientFactory, Arranger arranger, FdaClient fdaClient)
        {
            _logger = logger;
            _resourceProvider = resourceProvider;
            _httpClientFactory = httpClientFactory;
            _arranger = arranger;
            _fdaClient = fdaClient;
        }

        /// <summary>
        /// Adapt the project.
        /// </summary>
        public async Task AdoptAsync(Project project, string tlaFilename)
        {
            _logger.LogInformation("Adopt the project");

            var inputDocUrl = await _resourceProvider.CreateSignedUrlAsync(project.OSSSourceModel);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, tlaFilename);

            bool success = await _fdaClient.AdoptAsync(adoptionData);
            if (! success)
            {
                _logger.LogError($"Failed to process '{project.Name}' project.");
            }
            else
            {
                // rearrange generated data according to the parameters hash
                await _arranger.MoveProjectAsync(project, tlaFilename);

                _logger.LogInformation("Cache the project locally");

                // and now cache the generate stuff locally
                var projectLocalStorage = new ProjectStorage(project, _resourceProvider);
                await projectLocalStorage.EnsureLocalAsync(_httpClientFactory.CreateClient());
            }
        }

        public async Task<ProjectStateDTO> UpdateAsync(Project project, string tlaFilename, InventorParameters parameters)
        {
            _logger.LogInformation("Update the project");

            var inputDocUrl = await _resourceProvider.CreateSignedUrlAsync(project.OSSSourceModel);
            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, tlaFilename, parameters);

            bool success = await _fdaClient.AdoptAsync(adoptionData);
            if (! success)
            {
                _logger.LogError($"Failed to adopt {project.Name}");
                return null;
            }

            // rearrange generated data according to the parameters hash
            var hash = await _arranger.MoveViewablesAsync(project);

            _logger.LogInformation("Cache the project locally");

            // and now cache the generate stuff locally
            var projectStorage = new ProjectStorage(project, _resourceProvider);
            await projectStorage.EnsureViewablesAsync(_httpClientFactory.CreateClient(), hash);

            var localNames = projectStorage.GetLocalNames(hash);

            return new ProjectStateDTO
                    {
                        Svf = _resourceProvider.ToDataUrl(project.LocalNameProvider(hash).SvfDir),
                        Parameters = Json.DeserializeFile<InventorParameters>(localNames.Parameters)
                    };
        }
    }
}
