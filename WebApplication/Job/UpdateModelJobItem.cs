using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using System.Threading.Tasks;
using WebApplication.Processing;

namespace WebApplication.Job
{
    public class UpdateModelJobItem : JobItemBase
    {
        public InventorParameters Parameters { get; }

        public UpdateModelJobItem(ILogger logger, string projectId, InventorParameters parameters, ProjectWork projectWork)
            : base(logger, projectId, projectWork)
        {
            Parameters = parameters;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            Logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} started.");

            ProjectStateDTO updatedState = await ProjectWork.DoSmartUpdateAsync(Parameters, ProjectId);

            Logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} completed.");

            // send that we are done to client
            await resultSender.SendSuccessAsync(updatedState);
        }
    }
}
