using System.Threading.Tasks;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly CreateSvfDefinition _svfWork;

        public FdaClient(Publisher publisher)
        {
            _svfWork = new CreateSvfDefinition(publisher);
        }

        public async Task Initialize()
        {
            // create bundles and activities
            await _svfWork.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move pathname to configuration?
            //await GetThumbnailPublisher().Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move it to configuration?
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            await _svfWork.CleanUp();
            //await GetThumbnailPublisher().CleanUp();
        }

        public async Task GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            await _svfWork.ProcessIPT(inventorDocUrl, outputUrl);
        }
    }
}
