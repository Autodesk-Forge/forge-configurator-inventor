using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("parameters")]
    public class ParametersController : ControllerBase
    {
        private readonly UserResolver _userResolver;
        private readonly ILogger<ParametersController> _logger;

        public ParametersController(UserResolver userResolver, ILogger<ParametersController> logger)
        {
            _userResolver = userResolver;
            _logger = logger;
        }

        [HttpGet("{projectName}")]
        public async Task<InventorParameters> GetParameters(string projectName)
        {
            var projectStorage = await _userResolver.GetProjectStorageAsync(projectName);

            var localNames = projectStorage.GetLocalNames();
            var paramsFile = localNames.Parameters;
            if (! System.IO.File.Exists(paramsFile)) // TODO: unify it someday, not high priority
            {
                _logger.LogInformation($"Restoring missing parameters file for '{projectName}'");

                Directory.CreateDirectory(localNames.BaseDir);

                var bucket = await _userResolver.GetBucketAsync(tryToCreate: false);
                await bucket.DownloadFileAsync(projectStorage.GetOssNames().Parameters, paramsFile);
            }

            return Json.DeserializeFile<InventorParameters>(paramsFile);
        }
    }
}