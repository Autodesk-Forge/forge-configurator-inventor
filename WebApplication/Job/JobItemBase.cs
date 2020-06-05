using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApplication.Processing;

namespace WebApplication.Job
{
    public abstract class JobItemBase
    {
        protected ILogger Logger { get; }
        protected ProjectWork ProjectWork { get; }
        public string ProjectId { get; }
        public string Id { get; }

        protected JobItemBase(ILogger logger, string projectId, ProjectWork projectWork)
        {
            ProjectId = projectId;
            Id = Guid.NewGuid().ToString();
            ProjectWork = projectWork;
            Logger = logger;
        }

        public abstract Task ProcessJobAsync(IResultSender resultSender);
    }
}
