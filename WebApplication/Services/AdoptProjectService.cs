using Microsoft.Extensions.Logging;
using WebApplication.Definitions;

namespace WebApplication.Services
{
    public class AdoptProjectService
    {
        private readonly ILogger<AdoptProjectService> _logger;

        public AdoptProjectService(ILogger<AdoptProjectService> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// https://jira.autodesk.com/browse/INVGEN-45256
        /// </summary>
        /// <param name="payload">project configuration with parameters</param>
        public void AdoptProjectWithParameters(AdoptProjectWithParametersPayload payload)
        {
            _logger.LogInformation($"adopting project {payload.Name}");
        }
    }
}
