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
        private readonly IClientProxy _clientProxy;
        protected DefaultProjectsConfiguration DefaultPrjConfig { get; }
        protected ILogger Logger { get; }
        protected ProjectWork ProjectWork { get; }
        public string ProjectId { get; }
        public string Id { get; }

        protected JobItemBase(ILogger logger, string projectId, ProjectWork projectWork,
            DefaultProjectsConfiguration defaultProjectsConfiguration, IClientProxy clientProxy)
        {
            _clientProxy = clientProxy;
            ProjectId = projectId;
            Id = Guid.NewGuid().ToString();
            ProjectWork = projectWork;
            DefaultPrjConfig = defaultProjectsConfiguration;
            Logger = logger;
        }

        public abstract Task ProcessJobAsync();

        protected Task SendSuccessAsync()
        {
            return _clientProxy.SendAsync("onComplete");
        }

        protected Task SendSuccessAsync(object arg0)
        {
            return _clientProxy.SendAsync("onComplete", arg0);
        }

        protected Task SendSuccessAsync(object arg0, object arg1)
        {
            return _clientProxy.SendAsync("onComplete", arg0, arg1);
        }

        protected Task SendSuccessAsync(object arg0, object arg1, object arg2)
        {
            return _clientProxy.SendAsync("onComplete", arg0, arg1, arg2);
        }
    }
}