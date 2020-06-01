using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Definitions;
using WebApplication.Processing;

namespace WebApplication.Job
{
    public abstract class JobItemBase
    {
        public DefaultProjectsConfiguration DefaultPrjConfig { get; set; }
        public ProjectWork PrjWork { get; set; }
        public string ProjectId { get; }
        public string Id { get; }

        public JobItemBase(string projectId)
        {
            this.ProjectId = projectId;
            this.Id = Guid.NewGuid().ToString();
        }

        public abstract Task ProcessJobAsync(ILogger<JobProcessor> _logger, IHubContext<JobsHub> hubContext);
    }
}