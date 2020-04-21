using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Initializer
    {
        private readonly IForgeOSS _forge;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<Initializer> _logger;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly FdaClient _fdaClient;
        private readonly IHttpClientFactory _clientFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(IForgeOSS forge, ResourceProvider resourceProvider, ILogger<Initializer> logger,
                            FdaClient fdaClient, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            IHttpClientFactory clientFactory)
        {
            _forge = forge;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _fdaClient = fdaClient;
            _clientFactory = clientFactory;
            _defaultProjectsConfiguration = optionsAccessor.Value;
        }

        public async Task InitializeAsync()
        {
            using var scope = _logger.BeginScope("Init");

            // create bundles and activities
            await _fdaClient.InitializeAsync();

            _logger.LogInformation("Initializing base data");

            await _forge.CreateBucketAsync(_resourceProvider.BucketKey);
            _logger.LogInformation($"Bucket {_resourceProvider.BucketKey} created");

            var arranger = new Arranger(_forge, _clientFactory, _resourceProvider);

            // download default project files from the public location
            // specified by the appsettings.json
            var client = _clientFactory.CreateClient();

            foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
            {
                var projectUrl = defaultProjectConfig.Url;
                var tlaFilename = defaultProjectConfig.TopLevelAssembly;

                string[] urlParts = projectUrl.Split("/");
                string projectName = urlParts[^1];
                var project = new Project(projectName);

                _logger.LogInformation($"Download {projectUrl}");
                using (HttpResponseMessage response = await client.GetAsync(projectUrl))
                {
                    response.EnsureSuccessStatusCode();

                    _logger.LogInformation("Upload to the app bucket");

                    Stream stream = await response.Content.ReadAsStreamAsync();
                    await _forge.UploadObjectAsync(_resourceProvider.BucketKey, stream, project.OSSSourceModel);
                }

                _logger.LogInformation("Adopt the project");
                await AdoptAsync(arranger, project, tlaFilename);
            }

            _logger.LogInformation("Added default projects.");
        }

        public async Task ClearAsync()
        {
            try
            {
                await _forge.DeleteBucketAsync(_resourceProvider.BucketKey);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                _logger.LogInformation($"Nothing to delete because bucket {_resourceProvider.BucketKey} does not exists yet");
            }

            // delete bundles and activities
            await _fdaClient.CleanUpAsync();
        }

        /// <summary>
        /// Adapt the project.
        /// </summary>
        private async Task AdoptAsync(Arranger arranger, Project project, string tlaFilename)
        {
            var inputDocUrl = await _forge.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.OSSSourceModel);

            var adoptionData = await arranger.ForAdoption(inputDocUrl, tlaFilename);

            var status = await _fdaClient.AdoptAsync(adoptionData); // ER: think: it's a business logic, so it might not deal with low-level WI and status
            if (status.Status != Status.Success)
            {
                _logger.LogError($"Failed to adopt {project.Name}");
            }
            else
            {
                // rearrange generated data according to the parameters hash
                await arranger.Do(project);
            }
        }
    }
}
