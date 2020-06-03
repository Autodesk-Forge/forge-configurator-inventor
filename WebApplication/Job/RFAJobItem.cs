using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Job;
using WebApplication.Utilities;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private string temporaryUrl;

        public RFAJobItem(string projectId, string temporaryUrl)
            :base(projectId)
        {
            this.temporaryUrl = temporaryUrl;
        }

        public async override Task ProcessJobAsync(ILogger<JobProcessor> _logger, IHubContext<JobsHub> hubContext)
        {
            _logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} started.");

            Random r = new Random();
            int rand = r.Next(0, 30);
            Thread.Sleep(rand*100);

            _logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} completed.");

            // send that we are done to client
            await hubContext.Clients.All.SendAsync("onComplete", this.Id, temporaryUrl);
        }
    }
}