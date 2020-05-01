using Microsoft.AspNetCore.Mvc;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("parameters")]
    public class ParametersController : ControllerBase
    {
        private readonly ResourceProvider _resourceProvider;

        public ParametersController(ResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        [HttpGet("{projectName}")]
        public InventorParameters GetParameters(string projectName)
        {
            var projectStorage = _resourceProvider.GetProjectStorage(projectName);
            var paramsFile = projectStorage.GetLocalNames().Parameters;
            return Json.DeserializeFile<InventorParameters>(paramsFile);
        }
    }
}