using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;
using System.Threading.Tasks;

namespace WebApplication.Job
{
    public class UpdateModelJobItem : JobItemBase
    {
        public InventorParameters Parameters { get; }

        public UpdateModelJobItem(string projectId, InventorParameters parameters)
            : base(projectId)
        {
            Parameters = parameters;
        }

        public override async Task ProcessJobAsync(ILogger logger, IClientProxy clientProxy)
        {
            logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} started.");

            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({ProjectId})");
            }

            ProjectStateDTO updatedState = await PrjWork.DoSmartUpdateAsync(projectConfig, Parameters);

            logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} completed.");

            // send that we are done to client
            await clientProxy.SendAsync("onComplete", Id, updatedState);
        }
    }
}
