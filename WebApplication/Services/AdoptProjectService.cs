using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.State;

namespace WebApplication.Services
{
    public class AdoptProjectService
    {
        private readonly ILogger<AdoptProjectService> _logger;
        private readonly ProjectService _projectService;

        public AdoptProjectService(ILogger<AdoptProjectService> logger, ProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        /// <summary>
        /// https://jira.autodesk.com/browse/INVGEN-45256
        /// </summary>
        /// <param name="payload">project configuration with parameters</param>
        public string AdoptProjectWithParameters(AdoptProjectWithParametersPayload payload)
        {
            _logger.LogInformation($"adopting project {payload.Name}");

            var localFileName = Path.GetTempFileName();
            using (var client = new WebClient())
            {
                _logger.LogInformation($"downloading project from {payload.Url} to {localFileName}");
                client.DownloadFile(payload.Url, localFileName);
            }

            using FileStream stream = File.OpenRead(localFileName);

            _logger.LogInformation($"creating project {payload.Name}");
            var projectIdTask = _projectService.CreateProject(new NewProjectModel()
            {
                package = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                },
                root = payload.TopLevelAssembly
            });

            projectIdTask.Wait();

            return projectIdTask.Result;
        }
    }
}
