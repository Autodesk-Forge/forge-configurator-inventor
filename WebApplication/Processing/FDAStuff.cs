using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using IoConfigDemo;
using Microsoft.Extensions.Logging;

namespace WebApplication.Processing
{
    public class FDAStuff
    {
        private readonly DesignAutomationClient _fdaClient;
        private readonly ILogger<FDAStuff> _logger;
        private readonly BucketNameProvider _bucketNameProvider;

        public FDAStuff(DesignAutomationClient fdaClient, ILogger<FDAStuff> logger, BucketNameProvider bucketNameProvider)
        {
            _fdaClient = fdaClient;
            _logger = logger;
            _bucketNameProvider = bucketNameProvider;
        }

        public async Task Initialize()
        {
            // create bundles and activities
            var publisher = await GetSvfPublisher();
            await publisher.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
            //await GetThumbnailPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            var publisher = await GetSvfPublisher();
            await publisher.CleanUpAsync();
            //await GetThumbnailPublisher().CleanUpAsync();
        }

        private Task<Publisher> GetSvfPublisher() => Create<CreateSvfDefinition>();
        private Task<Publisher> GetThumbnailPublisher() => Create<CreateThumbnailDefinition>();

        private async Task<Publisher> Create<T>() where T : ForgeAppConfigBase, new()
        {
            return new Publisher(new T(), _fdaClient, _logger, await _bucketNameProvider.GetNicknameAsync());
        }

        public async Task GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            var publisher = await GetSvfPublisher();
            await new CreateSvfDefinition().ProcessIPT(publisher, inventorDocUrl, outputUrl);
        }
    }
}
