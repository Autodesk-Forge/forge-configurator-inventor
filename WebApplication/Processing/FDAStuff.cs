using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Logging;

namespace WebApplication.Processing
{
    public class FDAStuff
    {
        private readonly DesignAutomationClient _fdaClient;
        private readonly ILogger<FDAStuff> _logger;

        public FDAStuff(DesignAutomationClient fdaClient, ILogger<FDAStuff> logger)
        {
            _fdaClient = fdaClient;
            _logger = logger;
        }

        public async Task Initialize()
        {
            // create bundles and activities
            await GetSvfPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
            //await GetThumbnailPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            await GetSvfPublisher().CleanUpAsync();
            //await GetThumbnailPublisher().CleanUpAsync();
        }

        private Publisher GetSvfPublisher() => Create<CreateSvfDefinition>();
        private Publisher GetThumbnailPublisher() => Create<CreateThumbnailDefinition>();

        private Publisher Create<T>() where T : ForgeAppConfigBase, new()
        {
            return new Publisher(new T(), _fdaClient, _logger);
        }
    }
}
