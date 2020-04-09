using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Processing;

namespace IoConfigDemo
{
    public class Initializer
    {
        private readonly IForge _forge;
        private readonly BucketNameProvider _bucketNameProvider;
        private readonly ILogger<Initializer> _logger;

        /// <summary>
        /// Design Automation client.
        /// </summary>
        private DesignAutomationClient DesignAutomationClient
        {
            get
            {
                // TODO: can it be reused? creating new instance each time, just in case
                var httpMessageHandler = new ForgeHandler(Options.Create(_forge.Configuration))
                {
                    InnerHandler = new HttpClientHandler()
                };
                var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
                return new DesignAutomationClient(forgeService);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(IForge forge, BucketNameProvider bucketNameProvider, ILogger<Initializer> logger)
        {
            _forge = forge;
            _bucketNameProvider = bucketNameProvider;
            _logger = logger;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Initializing base data");
            await _forge.CreateBucket(_bucketNameProvider.BucketName);
            _logger.LogInformation($"Bucket {_bucketNameProvider.BucketName} created");
            
            await Task.WhenAll(
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, "Project1.zip"),
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, "Project2.zip"),
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, "Project3.zip")
            );
            _logger.LogInformation("Added empty projects.");

            // create bundles and activities
            var publisher = GetSvfPublisher();
            await publisher.PostAppBundleAsync(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
            await publisher.PublishActivityAsync();
        }

        public async Task Clear()
        {
            try
            {
                await _forge.DeleteBucket(_bucketNameProvider.BucketName);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                _logger.LogInformation($"Nothing to delete because bucket {_bucketNameProvider.BucketName} does not exists yet");
            }

            // delete bundles and activities
            var publisher = GetSvfPublisher();
            //await publisher.CleanExistingAppActivityAsync();
            // TODO: delete app bundle
        }

        private Publisher GetSvfPublisher() => new Publisher(new CreateSvfDefinition(), DesignAutomationClient);
        private Publisher GetThumbnailPublisher() => new Publisher(new CreateThumbnailDefinition(), DesignAutomationClient);
    }
}