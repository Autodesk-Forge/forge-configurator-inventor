using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Controllers;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private readonly string _hash;

        public RFAJobItem(string projectId, string hash)
            :base(projectId)
        {
            this._hash = hash;
        }

        public override async Task ProcessJobAsync(ILogger<JobProcessor> logger, IHubContext<JobsHub> hubContext)
        {
            logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} started.");
            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == this.ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({this.ProjectId})");
            }
            string rfaUrl = await PrjWork.GenerateRfaAsync(projectConfig, this._hash);

            logger.LogInformation($"ProcessJob (RFA) {this.Id} for project {this.ProjectId} completed. ({rfaUrl})");

            // send that we are done to client
            await hubContext.Clients.All.SendAsync("onComplete", rfaUrl);
        }
    }
}