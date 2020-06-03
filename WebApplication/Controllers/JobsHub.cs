using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Job;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub
    {
        private readonly ILogger<JobsHub> _logger;
        private readonly JobProcessor _jobProcessor;

        public JobsHub(JobProcessor jobProcessor, ILogger<JobsHub> logger)
        {
            _logger = logger;
            _jobProcessor = jobProcessor;
        }

        public void CreateUpdateJob(string projectId, InventorParameters parameters)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");
            // create job
            // add to jobprocessor (run in thread inside)
            _jobProcessor.AddNewJob(new UpdateModelJobItem(projectId, parameters));
        }

        public void CreateRFAJob(string projectId, string temporaryUrl)
        {
            _logger.LogInformation($"invoked CreateRFAJob, connectionId : {Context.ConnectionId}");
            // create job
            // add to jobprocessor (run in thread inside)
            _jobProcessor.AddNewJob(new RFAJobItem(projectId, temporaryUrl));
        }
    }
}
