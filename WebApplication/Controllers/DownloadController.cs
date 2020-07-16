using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("download")]
    public class DownloadController : ControllerBase
    {
        private readonly ILogger<DownloadController> _logger;
        private readonly UserResolver _userResolver;

        public DownloadController(ILogger<DownloadController> logger, UserResolver userResolver)
        {
            _logger = logger;
            _userResolver = userResolver;
        }

        [HttpGet("{projectName}/{hash}/model/{token?}")]
        public Task<RedirectResult> Model(string projectName, string hash, string token=null)
        {
            if (token != null)
                _userResolver.Token = token;

            return RedirectToOssObject(projectName, hash, (ossNames, isAssembly ) => ossNames.GetCurrentModel(isAssembly));
        }

        [HttpGet("{projectName}/{hash}/rfa/{token?}")]
        public Task<RedirectResult> RFA(string projectName, string hash, string token = null)
        {
            if (token != null)
                _userResolver.Token = token;

            return RedirectToOssObject(projectName, hash, (ossNames, _)=> ossNames.Rfa);
        }

        private async Task<RedirectResult> RedirectToOssObject(string projectName, string hash, Func<OSSObjectNameProvider, bool, string> nameExtractor)
        {
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName);
            
            var ossNameProvider = projectStorage.GetOssNames(hash);
            string ossObjectName = nameExtractor(ossNameProvider, projectStorage.IsAssembly);

            _logger.LogInformation($"Downloading '{ossObjectName}'");

            var bucket = await _userResolver.GetBucketAsync();
            var url = await bucket.CreateSignedUrlAsync(ossObjectName);

            // TODO: FIX: file will be downloaded as `cache-Wrench-3CEEF3FDD5135E1F5EF39BF000B62D673B5438FE-xxxxxx.zip`
            return Redirect(url);
        }
    }
}
