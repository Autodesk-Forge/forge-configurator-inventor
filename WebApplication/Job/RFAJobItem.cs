using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Job;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private string projectId;

        public RFAJobItem(string projectId)
            :base(projectId)
        {
        }

        public async override Task ProcessJobAsync(ILogger<JobProcessor> _logger, IHubContext<JobsHub> hubContext)
        {
            _logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} started.");

            Thread.Sleep(2000);

            _logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} completed.");

            // send that we are done to client
            await hubContext.Clients.All.SendAsync("onComplete", this.Id);
        }
    }
}