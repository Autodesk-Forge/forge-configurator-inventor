using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly CreateSvfDefinition _svfWork;
        private readonly CreateThumbnailDefinition _thumbnailWork;
        private readonly ExtractParametersDefinition _parametersWork;

        public FdaClient(Publisher publisher)
        {
            _svfWork = new CreateSvfDefinition(publisher);
            _thumbnailWork = new CreateThumbnailDefinition(publisher);
            _parametersWork = new ExtractParametersDefinition(publisher);
        }

        public async Task Initialize()
        {
            // create bundles and activities
            await _svfWork.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move pathname to configuration?
            await _thumbnailWork.Initialize(@"..\AppBundles\Output\CreateThumbnailPlugin.bundle.zip"); // TODO: move pathname to configuration?
            await _parametersWork.Initialize(@"..\AppBundles\Output\ExtractParametersPlugin.bundle.zip"); // TODO: move pathname to configuration?
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            await _svfWork.CleanUp();
            await _thumbnailWork.CleanUp();
            await _parametersWork.CleanUp();
        }

        public Task<WorkItemStatus> GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            return _svfWork.ProcessIpt(inventorDocUrl, outputUrl);
        }
    }
}
