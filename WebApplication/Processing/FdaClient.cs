using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly CreateSVF _svfWork;
        private readonly CreateThumbnail _thumbnailWork;
        private readonly ExtractParameters _parametersWork;
        private readonly AdoptProject _adoptWork;

        public FdaClient(Publisher publisher)
        {
            _svfWork = new CreateSVF(publisher);
            _thumbnailWork = new CreateThumbnail(publisher);
            _parametersWork = new ExtractParameters(publisher);
            _adoptWork = new AdoptProject(publisher);
        }

        public async Task Initialize()
        {
            // create bundles and activities
            await _svfWork.Initialize(@"..\AppBundles\Output\CreateSVFPlugin.bundle.zip"); // TODO: move pathname to configuration?
            await _thumbnailWork.Initialize(@"..\AppBundles\Output\CreateThumbnailPlugin.bundle.zip"); // TODO: move pathname to configuration?
            await _parametersWork.Initialize(@"..\AppBundles\Output\ExtractParametersPlugin.bundle.zip"); // TODO: move pathname to configuration?
            await _adoptWork.Initialize(null /* does not matter */);
        }

        public async Task CleanUp()
        {
            // delete bundles and activities
            await _svfWork.CleanUp();
            await _thumbnailWork.CleanUp();
            await _parametersWork.CleanUp();
            await _adoptWork.CleanUp();
        }

        public Task<WorkItemStatus> GenerateSVF(string inventorDocUrl, string outputUrl)
        {
            return _svfWork.ProcessIpt(inventorDocUrl, outputUrl);
        }
    }
}
