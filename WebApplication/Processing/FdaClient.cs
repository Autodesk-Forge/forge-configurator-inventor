using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Options;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly CreateSVF _svfWork;
        private readonly CreateThumbnail _thumbnailWork;
        private readonly ExtractParameters _parametersWork;
        private readonly AdoptProject _adoptWork;
        private readonly AppBundleZipPaths _paths;

        public FdaClient(Publisher publisher, IOptions<AppBundleZipPaths> appBundleZipPathsOptionsAccessor)
        {
            _svfWork = new CreateSVF(publisher);
            _thumbnailWork = new CreateThumbnail(publisher);
            _parametersWork = new ExtractParameters(publisher);
            _adoptWork = new AdoptProject(publisher);
            _paths = appBundleZipPathsOptionsAccessor.Value;
        }

        public async Task Initialize()
        {
            // create bundles and activities
            await _svfWork.Initialize(_paths.CreateSVF);
            await _thumbnailWork.Initialize(_paths.CreateThumbnail);
            await _parametersWork.Initialize(_paths.ExtractParameters);
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

        public Task<WorkItemStatus> Adopt(AdoptionData projectData)
        {
            return _adoptWork.Process(projectData);
        }
    }
}
