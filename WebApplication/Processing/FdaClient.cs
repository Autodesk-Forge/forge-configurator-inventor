using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly TransferData _transferData;
        private readonly CreateSVF _svfWork;
        private readonly CreateThumbnail _thumbnailWork;
        private readonly ExtractParameters _parametersWork;
        private readonly AdoptProject _adoptWork;
        private readonly AppBundleZipPaths _paths;
        private readonly UpdateParameters _updateParametersWork;

        public FdaClient(Publisher publisher, IOptions<AppBundleZipPaths> appBundleZipPathsOptionsAccessor)
        {
            _transferData = new TransferData(publisher);
            _svfWork = new CreateSVF(publisher);
            _thumbnailWork = new CreateThumbnail(publisher);
            _parametersWork = new ExtractParameters(publisher);
            _adoptWork = new AdoptProject(publisher);
            _updateParametersWork = new UpdateParameters(publisher);
            _paths = appBundleZipPathsOptionsAccessor.Value;
        }

        public async Task InitializeAsync()
        {
            // create bundles and activities
            await _transferData.InitializeAsync(_paths.EmptyExe);
            await _svfWork.InitializeAsync(_paths.CreateSVF);
            await _thumbnailWork.InitializeAsync(_paths.CreateThumbnail);
            await _parametersWork.InitializeAsync(_paths.ExtractParameters);
            await _updateParametersWork.InitializeAsync(_paths.UpdateParameters);
            await _adoptWork.InitializeAsync(null /* does not matter */);
        }

        public async Task CleanUpAsync()
        {
            // delete bundles and activities
            await _transferData.CleanUpAsync();
            await _svfWork.CleanUpAsync();
            await _thumbnailWork.CleanUpAsync();
            await _parametersWork.CleanUpAsync();
            await _updateParametersWork.CleanUpAsync();
            await _adoptWork.CleanUpAsync();
        }

        public Task<bool> AdoptAsync(AdoptionData projectData)
        {
            return _adoptWork.ProcessAsync(projectData);
        }
        public Task<bool> UpdateAsync(AdoptionData projectData) // TODO: should be UpdateData
        {
            return _adoptWork.ProcessAsync(projectData);
        }

        internal Task<bool> TransferAsync(string source, string target)
        {
            return _transferData.ProcessAsync(source, target);
        }
    }
}
