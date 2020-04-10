using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Logging;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly DesignAutomationClient _fdaClient;
        private readonly ILogger<FdaClient> _logger;
        private readonly BucketNameProvider _bucketNameProvider;
        private readonly CreateSvfDefinition _svfWork = new CreateSvfDefinition();
        private Publisher _publisher;

        public FdaClient(DesignAutomationClient fdaClient, ILogger<FdaClient> logger, BucketNameProvider bucketNameProvider)
        {
            _fdaClient = fdaClient;
            _logger = logger;
            _bucketNameProvider = bucketNameProvider;
        }

        public async Task Initialize()
        {
            // create bundles and activities
            var publisher = await GetPublisher();
            await publisher.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip", _svfWork); // TODO: move pathname to configuration?
            //await GetThumbnailPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
        }

        private async Task<Publisher> GetPublisher()
        {
            if (_publisher == null)
            {
                _publisher = new Publisher(_fdaClient, _logger, await _bucketNameProvider.GetNicknameAsync()); // TODO: find a proper way to resolve bucket name once. too many awaits
            }
            return _publisher;
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            await (await GetPublisher()).CleanUpAsync(_svfWork);
            //await GetThumbnailPublisher().CleanUpAsync();
        }

        public async Task GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            await _svfWork.ProcessIPT(await GetPublisher(), inventorDocUrl, outputUrl);
        }
    }
}
