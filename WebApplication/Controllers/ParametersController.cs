using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Definitions;
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
        public ParametersDTO GetParametersAsync(string projectName)
        {
            ProjectStorage projectStorage = _resourceProvider.GetProjectStorage(projectName);
            var inventorParameters = Json.DeserializeFile<InventorParameters>(projectStorage.LocalNames.Parameters);

            return ToDTO(inventorParameters);
        }

        /// <summary>
        /// Convert `parameters.json` format into format expected by client-side.
        /// </summary>
        private static ParametersDTO ToDTO(InventorParameters data)
        {
            return new ParametersDTO
                    {
                        Parameters = data.Select(pair =>
                                            {
                                                (string name, InventorParameter param) = pair;
                                                return new ParameterDTO
                                                {
                                                    Name = name,
                                                    Value = param.Value,
                                                    AllowedValues = param.Values,
                                                    Units = param.Unit,
                                                    Type = "NYI" // and not sure where to get it right now
                                                };
                                            }).ToArray()
                    };
                }
    }
}