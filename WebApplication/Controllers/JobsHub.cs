using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Processing;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub, IResultSender
    {
        /// <summary>
        /// Remote method name to be called on success.
        /// </summary>
        private const string OnComplete = "onComplete";

        /// <summary>
        /// Remote method name to be called on failure.
        /// </summary>
        private const string OnError = "onError";

        private readonly ILogger<JobsHub> _logger;
        private readonly ProjectWork _projectWork;
        private readonly LinkGenerator _linkGenerator;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;

        public JobsHub(ILogger<JobsHub> logger, IOptions<DefaultProjectsConfiguration> options, ProjectWork projectWork, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _projectWork = projectWork;
            _linkGenerator = linkGenerator;
            _defaultProjectsConfiguration = options.Value;
        }

        public Task CreateUpdateJob(string projectId, InventorParameters parameters)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");

            // create job and run it
            var job = new UpdateModelJobItem(_logger, projectId, parameters, _projectWork, _defaultProjectsConfiguration); // TODO: is it correct to use `Clients.All`?
            return RunJobAsync(job);
        }

        public Task CreateRFAJob(string projectId, string hash)
        {
            _logger.LogInformation($"invoked CreateRFAJob, connectionId : {Context.ConnectionId}");

            // create job and run it
            var job = new RFAJobItem(_logger, projectId, hash, _projectWork, _defaultProjectsConfiguration, _linkGenerator);
            return RunJobAsync(job);
        }

        public async Task RunJobAsync(JobItemBase job)
        {
            try
            {
                await job.ProcessJobAsync(this as IResultSender);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for {job.Id}");
                await Clients.All.SendAsync(OnError, job.Id, e.Message);
            }
        }

        public Task SendSuccess0Async()
        {
            return Clients.All.SendAsync(OnComplete);
        }

        public Task SendSuccess1Async(object arg0)
        {
            return Clients.All.SendAsync(OnComplete, arg0);
        }

        public Task SendSuccess2Async(object arg0, object arg1)
        {
            return Clients.All.SendAsync(OnComplete, arg0, arg1);
        }

        public Task SendSuccess3Async(object arg0, object arg1, object arg2)
        {
            return Clients.All.SendAsync(OnComplete, arg0, arg1, arg2);
        }
    }
}
