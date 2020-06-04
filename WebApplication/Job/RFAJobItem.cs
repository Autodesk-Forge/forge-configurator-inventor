using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Definitions;
using WebApplication.Processing;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private readonly string _hash;

        public RFAJobItem(ILogger logger, string projectId, string hash, ProjectWork projectWork,
            DefaultProjectsConfiguration defaultProjectsConfiguration, IClientProxy clientProxy)
            : base(logger, projectId, projectWork, defaultProjectsConfiguration, clientProxy)
        {
            _hash = hash;
        }

        public override async Task ProcessJobAsync()
        {
            Logger.LogInformation($"ProcessJob (RFA) {Id} for project {ProjectId} started.");
            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({ProjectId})");
            }

            string rfaUrl = await ProjectWork.GenerateRfaAsync(projectConfig, _hash);
            Logger.LogInformation($"ProcessJob (RFA) {Id} for project {ProjectId} completed. ({rfaUrl})");

            // send that we are done to client
            await SendSuccessAsync(rfaUrl);
        }
    }
}