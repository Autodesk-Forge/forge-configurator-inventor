using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using WebApplication.Job;

namespace WebApplication.Controllers
{
    public class UpdateJobHub : Hub
    {
        IJobProcessor _jobProcessor;

        public UpdateJobHub(IJobProcessor jobProcessor)
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
