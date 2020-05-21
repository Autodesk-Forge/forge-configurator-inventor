using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProjectWork _projectWork;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(IForgeOSS forge, ResourceProvider resourceProvider, ILogger<Initializer> logger,
                            FdaClient fdaClient, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            IHttpClientFactory httpClientFactory, ProjectWork projectWork)
        {
            _forge = forge;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _fdaClient = fdaClient;
            _httpClientFactory = httpClientFactory;
            _projectWork = projectWork;
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

            // download default project files from the public location
            // specified by the appsettings.json
            var httpClient = _httpClientFactory.CreateClient();

            foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
            {
                var projectUrl = defaultProjectConfig.Url;
                var project = _resourceProvider.GetProject(defaultProjectConfig.Name);

                _logger.LogInformation($"Launching 'TransferData' for {projectUrl}");
                string signedUrl = await _forge.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.OSSSourceModel, ObjectAccess.Write);
                // TransferData from s3 to oss
                await _projectWork.FileTransferAsync(projectUrl, signedUrl);
                _logger.LogInformation($"'TransferData' for {projectUrl} is done.");

                await _projectWork.AdoptAsync(defaultProjectConfig);
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

            // cleanup locally cached files
            Directory.Delete(_resourceProvider.LocalRootName, true);
        }
    }
}
