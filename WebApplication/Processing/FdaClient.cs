using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;

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

        public async Task InitializeAsync()
        {
            // create bundles and activities
            await _svfWork.InitializeAsync(_paths.CreateSVF);
            await _thumbnailWork.InitializeAsync(_paths.CreateThumbnail);
            await _parametersWork.InitializeAsync(_paths.ExtractParameters);
            await _adoptWork.InitializeAsync(null /* does not matter */);
        }

        public async Task CleanUpAsync()
        {
            // delete bundles and activities
            await _svfWork.CleanUpAsync();
            await _thumbnailWork.CleanUpAsync();
            await _parametersWork.CleanUpAsync();
            await _adoptWork.CleanUpAsync();
        }

        public Task<WorkItemStatus> AdoptAsync(AdoptionData projectData)
        {
            return _adoptWork.ProcessAsync(projectData);
        }
    }
}
