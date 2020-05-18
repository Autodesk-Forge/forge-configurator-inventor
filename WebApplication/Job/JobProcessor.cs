using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication.Job
{
    public class JobProcessor
    {
        private readonly IHubContext<JobsHub> _hubContext;

        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<JobProcessor> _logger;
        private readonly ProjectWork _projectWork;

        public JobProcessor(IHubContext<JobsHub> hubContext, IOptions<DefaultProjectsConfiguration> optionsAccessor, 
                            ResourceProvider resourceProvider, ILogger<JobProcessor> logger, ProjectWork projectWork)
        {
            _hubContext = hubContext;

            _defaultProjectsConfiguration = optionsAccessor.Value;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _projectWork = projectWork;
        }

        public Task AddNewJob(JobItem job)
        {
            return ProcessJobAsync(job);
        }

        private async Task ProcessJobAsync(JobItem job)
        {
            var projectConfig = _defaultProjectsConfiguration.Projects.FirstOrDefault(cfg => cfg.Name == job.ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({job.ProjectId})");
            }

            var hash = Crypto.GenerateHashString(job.Data); // TODO: need to ensure JSON is the same each time
            _logger.LogInformation(job.Data);

            var project = _resourceProvider.GetProject(projectConfig.Name);
            var localNames = project.LocalNameProvider(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir))
            {
                _logger.LogInformation($"Found cached data corresponded to {hash}");
            }
            else
            {
                var parameters = JsonSerializer.Deserialize<InventorParameters>(job.Data);

                // TODO: what to do on processing errors?
                await _projectWork.UpdateAsync(project, parameters, projectConfig.TopLevelAssembly);
            }

            // send that we are done to client
            await _hubContext.Clients.All.SendAsync("onComplete", job.Id);
        }
    }
}
