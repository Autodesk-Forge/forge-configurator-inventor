using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<ActionResult> Model(string projectName, string hash)
        {
            Project project = _resourceProvider.GetProject(projectName);
            string modelOssName = project.OssNameProvider(hash).CurrentModel;

            _logger.LogInformation($"Downloading '{modelOssName}'");

            var url = await _forgeOSS.CreateSignedUrlAsync(_resourceProvider.BucketKey, modelOssName);

            // TODO: FIX: file will be downloaded as `cache-Wrench-3CEEF3FDD5135E1F5EF39BF000B62D673B5438FE-model.zip`
            return Redirect(url);
        }
    }
}
