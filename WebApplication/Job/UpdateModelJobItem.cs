using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Controllers;
using System.Threading.Tasks;

namespace WebApplication.Job
{
    public class UpdateModelJobItem : JobItemBase
    {
        public InventorParameters Parameters { get; }

        public UpdateModelJobItem(string projectId, InventorParameters parameters)
            : base(projectId)
        {
            this.Parameters = parameters;
        }

        public async override Task ProcessJobAsync(ILogger<JobProcessor> _logger, IHubContext<JobsHub> hubContext)
        {
            _logger.LogInformation($"ProcessJob (Update) {this.Id} for project {this.ProjectId} started.");

            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == this.ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({this.ProjectId})");
            }

            ProjectStateDTO updatedState = await PrjWork.DoSmartUpdateAsync(projectConfig, this.Parameters);

            _logger.LogInformation($"ProcessJob (Update) {this.Id} for project {this.ProjectId} completed.");

            // send that we are done to client
            await hubContext.Clients.All.SendAsync("onComplete", this.Id, updatedState);
        }
    }
}
