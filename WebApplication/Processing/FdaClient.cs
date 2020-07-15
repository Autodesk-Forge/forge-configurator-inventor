using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly TransferData _transferData;
        private readonly CreateSVF _svfWork;
        private readonly CreateSAT _satWork;
        private readonly CreateRFA _rfaWork;
        private readonly CreateThumbnail _thumbnailWork;
        private readonly ExtractParameters _parametersWork;
        private readonly AdoptProject _adoptAssemblyWork;
        private readonly AdoptProject _adoptPartWork;
        private readonly UpdateProject _updateProjectWork;
        private readonly AppBundleZipPaths _paths;
        private readonly UpdateParameters _updateParametersWork;
        private readonly ILogger<FdaClient> _logger;

        public FdaClient(Publisher publisher, IOptions<AppBundleZipPaths> appBundleZipPathsOptionsAccessor, ILogger<FdaClient> logger)
        {
            _transferData = new TransferData(publisher);
            _svfWork = new CreateSVF(publisher);
            _satWork = new CreateSAT(publisher);
            _rfaWork = new CreateRFA(publisher);
            _thumbnailWork = new CreateThumbnail(publisher);
            _parametersWork = new ExtractParameters(publisher);
            _adoptAssemblyWork = new AdoptProject(publisher, true);
            _adoptPartWork = new AdoptProject(publisher, false);
            _updateParametersWork = new UpdateParameters(publisher);
            _updateProjectWork = new UpdateProject(publisher);
            _paths = appBundleZipPathsOptionsAccessor.Value;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // create bundles and activities
            await _transferData.InitializeAsync(_paths.EmptyExe);
            await _svfWork.InitializeAsync(_paths.CreateSVF);
            await _satWork.InitializeAsync(_paths.CreateSAT);
            await _rfaWork.InitializeAsync(_paths.CreateRFA);
            await _thumbnailWork.InitializeAsync(_paths.CreateThumbnail);
            await _parametersWork.InitializeAsync(_paths.ExtractParameters);
            await _updateParametersWork.InitializeAsync(_paths.UpdateParameters);
            await _adoptAssemblyWork.InitializeAsync(null /* does not matter */);
            await _adoptPartWork.InitializeAsync(null /* does not matter */);
            await _updateProjectWork.InitializeAsync(null /* does not matter */);
        }

        public async Task CleanUpAsync()
        {
            // delete bundles and activities
            await _transferData.CleanUpAsync();
            await _svfWork.CleanUpAsync();
            await _satWork.CleanUpAsync();
            await _rfaWork.CleanUpAsync();
            await _thumbnailWork.CleanUpAsync();
            await _parametersWork.CleanUpAsync();
            await _updateParametersWork.CleanUpAsync();
            await _adoptAssemblyWork.CleanUpAsync();
            await _adoptPartWork.CleanUpAsync();
            await _updateProjectWork.CleanUpAsync();
        }

        public Task<ProcessingResult> AdoptAsync(AdoptionData projectData)
        {
            if (projectData.IsAssembly)
                return _adoptAssemblyWork.ProcessAsync(projectData);

            return _adoptPartWork.ProcessAsync(projectData);
        }

        public Task<ProcessingResult> UpdateAsync(UpdateData projectData)
        {
            return _updateProjectWork.ProcessAsync(projectData);
        }

        internal Task<ProcessingResult> TransferAsync(string source, string target)
        {
            return _transferData.ProcessAsync(source, target);
        }

        internal async Task<ProcessingResult> GenerateRfa(ProcessingArgs satData, ProcessingArgs rfaData)
        {
            ProcessingResult satResult = await _satWork.ProcessAsync(satData);
            if (!satResult.Success)
            {
                satResult.ErrorMessage = "Failed to generate SAT file";
                return satResult;
            }

            ProcessingResult rfaResult = await _rfaWork.ProcessAsync(rfaData);
            if (!rfaResult.Success)
            {
                rfaResult.ErrorMessage = "Failed to generate RFA file";
                return rfaResult;
            }

            return rfaResult;
        }
    }
}
