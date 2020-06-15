using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("download")]
    public class DownloadController : ControllerBase
    {
        private readonly IForgeOSS _forgeOSS;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(IForgeOSS forgeOSS, ResourceProvider resourceProvider, ILogger<DownloadController> logger)
        {
            _forgeOSS = forgeOSS;
            _resourceProvider = resourceProvider;
            _logger = logger;
        }

        [HttpGet("{projectName}/{hash}/model")]
        public Task<RedirectResult> Model(string projectName, string hash)
        {
            return RedirectToOssObject(projectName, hash, ossNames => ossNames.CurrentModel);
        }

        [HttpGet("{projectName}/{hash}/rfa")]
        public Task<RedirectResult> RFA(string projectName, string hash)
        {
            return RedirectToOssObject(projectName, hash, ossNames => ossNames.Rfa);
        }

        private async Task<RedirectResult> RedirectToOssObject(string projectName, string hash, Func<OSSObjectNameProvider, string> nameExtractor )
        {
            Project project = _resourceProvider.GetProject(projectName);

            var ossNameProvider = project.OssNameProvider(hash);
            string ossObjectName = nameExtractor(ossNameProvider);

            _logger.LogInformation($"Downloading '{ossObjectName}'");

            var url = await _forgeOSS.CreateSignedUrlAsync(_resourceProvider.BucketKey, ossObjectName);

            // TODO: FIX: file will be downloaded as `cache-Wrench-3CEEF3FDD5135E1F5EF39BF000B62D673B5438FE-xxxxxx.zip`
            return Redirect(url);
        }
    }
}
