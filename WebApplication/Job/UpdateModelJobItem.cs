using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using System.Threading.Tasks;
using WebApplication.Processing;

namespace WebApplication.Job
{
    public class UpdateModelJobItem : JobItemBase
    {
        public InventorParameters Parameters { get; }

        public UpdateModelJobItem(ILogger logger, string projectId, InventorParameters parameters,
            ProjectWork projectWork,
            DefaultProjectsConfiguration defaultProjectsConfiguration, IClientProxy clientProxy)
            : base(logger, projectId, projectWork, defaultProjectsConfiguration, clientProxy)
        {
            Parameters = parameters;
        }

        public override async Task ProcessJobAsync()
        {
            Logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} started.");

            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({ProjectId})");
            }

            ProjectStateDTO updatedState = await ProjectWork.DoSmartUpdateAsync(projectConfig, Parameters);

            Logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} completed.");

            // send that we are done to client
            await SendSuccessAsync(Id, updatedState);
        }
    }
}
