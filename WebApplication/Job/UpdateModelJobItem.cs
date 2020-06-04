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
            DefaultProjectsConfiguration defaultProjectsConfiguration)
            : base(logger, projectId, projectWork, defaultProjectsConfiguration)
        {
            Parameters = parameters;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
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
            await resultSender.SendSuccess2Async(Id, updatedState);
        }
    }
}
