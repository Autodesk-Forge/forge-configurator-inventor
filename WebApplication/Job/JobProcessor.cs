using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Definitions;
using WebApplication.Processing;

namespace WebApplication.Job
{
    public class JobProcessor
    {
        private readonly IHubContext<JobsHub> _hubContext;

        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly ILogger<JobProcessor> _logger;
        private readonly ProjectWork _projectWork;

        public JobProcessor(IHubContext<JobsHub> hubContext, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            ILogger<JobProcessor> logger, ProjectWork projectWork)
        {
            _hubContext = hubContext;

            _defaultProjectsConfiguration = optionsAccessor.Value;
            _logger = logger;
            _projectWork = projectWork;
        }

        public Task AddNewJob(JobItem job)
        {
            return ProcessJobAsync(job);
        }

        private async Task ProcessJobAsync(JobItem job)
        {
            _logger.LogInformation($"ProcessJob {job.Id} for project {job.ProjectId} started.");

            var projectConfig = _defaultProjectsConfiguration.Projects.FirstOrDefault(cfg => cfg.Name == job.ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({job.ProjectId})");
            }

            ProjectStateDTO updatedState = await _projectWork.DoSmartUpdateAsync(projectConfig, job.Parameters);

            _logger.LogInformation($"ProcessJob {job.Id} for project {job.ProjectId} completed.");

            // send that we are done to client
            await _hubContext.Clients.All.SendAsync("onComplete", job.Id, updatedState);
        }
    }
}
