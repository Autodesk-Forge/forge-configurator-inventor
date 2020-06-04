using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Processing;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub
    {
        private readonly ILogger<JobsHub> _logger;
        private readonly ProjectWork _projectWork;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;

        public JobsHub(ILogger<JobsHub> logger, IOptions<DefaultProjectsConfiguration> options, ProjectWork projectWork)
        {
            _logger = logger;
            _projectWork = projectWork;
            _defaultProjectsConfiguration = options.Value;
        }

        public Task CreateUpdateJob(string projectId, InventorParameters parameters)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");

            // create job and run it
            var job = new UpdateModelJobItem(_logger, projectId, parameters, _projectWork, _defaultProjectsConfiguration, Clients.All); // TODO: is it correct to use `Clients.All`?
            return RunJobAsync(job);
        }

        public Task CreateRFAJob(string projectId, string hash)
        {
            _logger.LogInformation($"invoked CreateRFAJob, connectionId : {Context.ConnectionId}");

            // create job and run it
            var job = new RFAJobItem(_logger, projectId, hash, _projectWork, _defaultProjectsConfiguration, Clients.All);
            return RunJobAsync(job);
        }

        public async Task RunJobAsync(JobItemBase job)
        {
            try
            {
                await job.ProcessJobAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for {job.Id}");
                await Clients.All.SendAsync("onError", job.Id, e.Message);
            }
        }
    }
}
