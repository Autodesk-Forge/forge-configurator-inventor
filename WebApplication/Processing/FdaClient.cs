using System.Threading.Tasks;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly CreateSvfDefinition _svfWork = new CreateSvfDefinition();
        private readonly Publisher _publisher;

        public FdaClient(Publisher publisher)
        {

            _publisher = publisher;
        }

        public async Task Initialize()
        {
            // create bundles and activities
            await _publisher.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip", _svfWork); // TODO: move pathname to configuration?
            //await GetThumbnailPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            await _publisher.CleanUpAsync(_svfWork);
            //await GetThumbnailPublisher().CleanUpAsync();
        }

        public async Task GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            await _svfWork.ProcessIPT(_publisher, inventorDocUrl, outputUrl);
        }
    }
}
