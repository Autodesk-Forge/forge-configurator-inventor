using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Utilities;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private readonly string hash;

        public RFAJobItem(string projectId, string hash)
            :base(projectId)
        {
            this.hash = hash;
        }

        public async override Task ProcessJobAsync(ILogger<JobProcessor> _logger, IHubContext<JobsHub> hubContext)
        {
            _logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} started.");
            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == this.ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({this.ProjectId})");
            }
            string rfaUrl = await PrjWork.GenerateRfaAsync(projectConfig, this.hash);

            _logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} completed.");

            // send that we are done to client
            await hubContext.Clients.All.SendAsync("onComplete", rfaUrl);
        }
    }
}