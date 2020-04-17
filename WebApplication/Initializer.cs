using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.DesignAutomation.Model;
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
            using var scope = _logger.BeginScope("Init");

            // create bundles and activities
            await _fdaClient.Initialize();

            _logger.LogInformation("Initializing base data");

            await _forge.CreateBucket(_resourceProvider.BucketName);
            _logger.LogInformation($"Bucket {_resourceProvider.BucketName} created");

            // download default project files from the public location
            // specified by the appsettings.json
            using (var client = new HttpClient())
            {
                string[] defaultProjects = _configuration.GetSection("DefaultProjects:Files").Get<string[]>();
                string[] tlaFilenames = _configuration.GetSection("DefaultProjects:TopLevelAssemblies").Get<string[]>();
                if (defaultProjects.Length != tlaFilenames.Length)
                {
                    throw new Exception("Default projects are not in sync with TLA names");
                }

                var bucketKey = _resourceProvider.BucketName;

                for (var i = 0; i < defaultProjects.Length; i++)
                {
                    var projectUrl = defaultProjects[i];
                    var tlaFilename = tlaFilenames[i];

                    string[] urlParts = projectUrl.Split("/");
                    string projectName = urlParts[^1];
                    var project = new Project(projectName);

                    _logger.LogInformation($"Download {projectUrl}");
                    using (HttpResponseMessage response = await client.GetAsync(projectUrl).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        _logger.LogInformation("Upload to the app bucket");

                        Stream stream = await response.Content.ReadAsStreamAsync();
                        await _forge.UploadObject(bucketKey, stream, project.OSSSourceModel);
                    }

                    var documentUrl = await _forge.CreateSignedUrl(bucketKey, project.OSSSourceModel);

                    _logger.LogInformation("Adopt the project");
                    var arranger = new Arranger(_forge, bucketKey);

                    var adoptionData = await arranger.ForAdoption(documentUrl, tlaFilename);

                    var status = await _fdaClient.Adopt(adoptionData); // ER: think - it's a business logic, so it might not deal with low-level WI and status
                    if (status.Status != Status.Success)
                    {
                        _logger.LogError($"Failed to adopt {projectUrl}");
                    }
                    else
                    {
                        // rearrange generated data according to the parameters hash
                        await arranger.Do(project);
                    }
                }
            }

            _logger.LogInformation("Added default projects.");
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
