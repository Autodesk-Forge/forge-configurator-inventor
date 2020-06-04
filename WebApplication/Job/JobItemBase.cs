using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
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

        protected JobItemBase(string projectId)
        {
            ProjectId = projectId;
            Id = Guid.NewGuid().ToString();
        }

        public abstract Task ProcessJobAsync(ILogger logger, IClientProxy clientProxy);
    }
}