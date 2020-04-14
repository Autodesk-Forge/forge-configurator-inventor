using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Initializer
    {
        private readonly IForge _forge;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<Initializer> _logger;
        private readonly IConfiguration _configuration;
        private readonly FdaClient _fdaClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(IForge forge, ResourceProvider resourceProvider, ILogger<Initializer> logger, FdaClient fdaClient, IConfiguration configuration)
        {
            _forge = forge;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _fdaClient = fdaClient;
            _configuration = configuration;
        }

        public async Task Initialize()
        {
            _logger.LogInformation($"Initializing base data");
            await _forge.CreateBucket(_bucketNameProvider.BucketName);
            _logger.LogInformation($"Bucket {_bucketNameProvider.BucketName} created");

            // download default project files from the public location
            // specified by the appsettings.json
            var client = new HttpClient();
            string[] defaultProjects = _configuration.GetSection("DefaultProjects:Files").Get<string[]>();
            foreach (var projectUrl in defaultProjects)
            {
                HttpResponseMessage response = await client.GetAsync(projectUrl);
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                string[] urlParts = projectUrl.Split("/");
                string projectName = urlParts[urlParts.Length - 1];
                await _forge.UploadObject(_bucketNameProvider.BucketName, stream, new Project(projectName).SourceModel);
            }
            _logger.LogInformation($"Added default projects.");

            // create bundles and activities
            await _fdaClient.Initialize();
        }

        public async Task Clear()
        {
            try
            {
                await _forge.DeleteBucket(_resourceProvider.BucketName);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                _logger.LogInformation($"Nothing to delete because bucket {_resourceProvider.BucketName} does not exists yet");
            }

            // delete bundles and activities
            await _fdaClient.CleanUp();
        }
    }
}
