using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
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
        private readonly FdaClient _fdaClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(IForge forge, ResourceProvider resourceProvider, ILogger<Initializer> logger, FdaClient fdaClient)
        {
            _forge = forge;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _fdaClient = fdaClient;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Initializing base data");
            await _forge.CreateBucket(_resourceProvider.BucketName);
            _logger.LogInformation($"Bucket {_resourceProvider.BucketName} created");
            
            await Task.WhenAll(
                _forge.CreateEmptyObject(_resourceProvider.BucketName, "Project1.zip"),
                _forge.CreateEmptyObject(_resourceProvider.BucketName, "Project2.zip"),
                _forge.CreateEmptyObject(_resourceProvider.BucketName, "Project3.zip")
            );
            _logger.LogInformation("Added empty projects.");

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
