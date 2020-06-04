using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private readonly string _hash;

        public RFAJobItem(string projectId, string hash): base(projectId)
        {
            _hash = hash;
        }

        public override async Task ProcessJobAsync(ILogger logger, IClientProxy clientProxy)
        {
            logger.LogInformation($"ProcessJob (RFA) {Id} for project {ProjectId} started.");
            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({ProjectId})");
            }
            string rfaUrl = await PrjWork.GenerateRfaAsync(projectConfig, _hash);

            logger.LogInformation($"ProcessJob (RFA) {Id} for project {ProjectId} completed. ({rfaUrl})");

            // send that we are done to client
            await clientProxy.SendAsync("onComplete", rfaUrl);
        }
    }
}