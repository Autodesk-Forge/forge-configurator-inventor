using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IoConfigDemo
{
    public class Initializer
    {
        private readonly IForge _forge;
        private readonly BucketNameProvider _bucketNameProvider;
        private readonly ILogger<Initializer> _logger;
        private readonly IConfiguration _configuration;
        public Initializer(IForge forge, BucketNameProvider bucketNameProvider, ILogger<Initializer> logger, IConfiguration configuration)
        {
            _forge = forge;
            _bucketNameProvider = bucketNameProvider;
            _logger = logger;
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
            string file = null;
            int fileIndex = 0;
            // read the config file
            string location = _configuration.GetValue<string>("DefaultProjects:Location");
            while ((file = _configuration.GetValue<string>("DefaultProjects:Files:" + fileIndex.ToString())) != null)
            {
                HttpResponseMessage response = await client.GetAsync(location + "/" + file);
                if (response.IsSuccessStatusCode) {
                    Stream stream = await response.Content.ReadAsStreamAsync();
                    await _forge.UploadObject(_bucketNameProvider.BucketName, stream, file);
                }
                fileIndex++;
            }
            _logger.LogInformation($"Added default projects.");
        }

        public async Task Clear()
        {
            try {
                await _forge.DeleteBucket(_bucketNameProvider.BucketName);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            } catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound) {
                _logger.LogInformation($"Nothing to delete because bucket {_bucketNameProvider.BucketName} does not exists yet");
            }
            
        }
    }
}