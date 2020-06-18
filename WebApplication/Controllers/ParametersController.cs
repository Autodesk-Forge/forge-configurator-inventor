using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public ParametersController(UserResolver userResolver)
        {
            _userResolver = userResolver;
        }

        [HttpGet("{projectName}")]
        public async Task<InventorParameters> GetParameters(string projectName)
        {
            var projectStorage = await _userResolver.GetProjectStorageAsync(projectName);
            var paramsFile = projectStorage.GetLocalNames().Parameters;
            return Json.DeserializeFile<InventorParameters>(paramsFile);
        }
    }
}