using Microsoft.AspNetCore.SignalR;
using WebApplication.Job;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub
    {
        readonly JobProcessor _jobProcessor;

        public JobsHub(JobProcessor jobProcessor)
        {
            _jobProcessor = jobProcessor;
        }

        public void CreateJob(string projectId, string data)
        {
            // create job
            // add to jobprocessor (run in thread inside)
            _jobProcessor.AddNewJob(new JobItem(projectId, data));
        }
    }
}
