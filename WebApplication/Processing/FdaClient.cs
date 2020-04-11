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
        private readonly ResourceProvider _resourceProvider;
        private readonly CreateSvfDefinition _svfWork = new CreateSvfDefinition();
        private Publisher _publisher;

        public FdaClient(DesignAutomationClient fdaClient, ILogger<FdaClient> logger, ResourceProvider resourceProvider)
        {
            _fdaClient = fdaClient;
            _logger = logger;
            _resourceProvider = resourceProvider;
        }

        public async Task Initialize()
        {
            // create bundles and activities
            var publisher = await CreatePublisher();
            await publisher.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip", _svfWork); // TODO: move pathname to configuration?
            //await GetThumbnailPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
        }

        private async Task<Publisher> CreatePublisher()
        {
            var nicknameAsync = await _resourceProvider.GetNicknameAsync();
            _publisher = new Publisher(_fdaClient, _logger, nicknameAsync);
            return _publisher;
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            var publisher = await CreatePublisher();
            await publisher.CleanUpAsync(_svfWork);
            //await GetThumbnailPublisher().CleanUpAsync();
        }

        public async Task GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            await _svfWork.ProcessIPT(await CreatePublisher(), inventorDocUrl, outputUrl);
        }
    }
}
