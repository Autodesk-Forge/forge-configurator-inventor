using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using WebApplication.Definitions;
using WebApplication.Processing;

namespace WebApplication.Job
{
    internal class RFAJobItem : JobItemBase
    {
        private readonly string _hash;
        private readonly LinkGenerator _linkGenerator;

        public RFAJobItem(ILogger logger, string projectId, string hash, ProjectWork projectWork,
            DefaultProjectsConfiguration defaultProjectsConfiguration, LinkGenerator linkGenerator)
            : base(logger, projectId, projectWork, defaultProjectsConfiguration)
        {
            _hash = hash;
            _linkGenerator = linkGenerator;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            Logger.LogInformation($"ProcessJob (RFA) {Id} for project {ProjectId} started.");
            var projectConfig = DefaultPrjConfig.Projects.FirstOrDefault(cfg => cfg.Name == ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({ProjectId})");
            }

            await ProjectWork.GenerateRfaAsync(projectConfig, _hash);
            Logger.LogInformation($"ProcessJob (RFA) {Id} for project {ProjectId} completed.");

            // TODO: this url can be generated right away... we can simply acknowledge that OSS file is ready,
            // without generating URL here
            string rfaUrl = _linkGenerator.GetPathByAction(controller: "Download",
                                                            action: "RFA",
                                                            values: new {projectName = ProjectId, hash = _hash});

            // send resulting URL to the client
            await resultSender.SendSuccessAsync(rfaUrl);
        }
    }
}