using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebApplication.Job;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub
    {
        private readonly ILogger<JobsHub> _logger;
        JobProcessor _jobProcessor;

        public JobsHub(JobProcessor jobProcessor, ILogger<JobsHub> logger)
        {
            _logger = logger;
            _jobProcessor = jobProcessor;
        }

        public void CreateJob(string projectId, string data)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");
            // create job
            // add to jobprocessor (run in thread inside)
            _jobProcessor.AddNewJob(new JobItem(projectId, data));
        }
    }
}
