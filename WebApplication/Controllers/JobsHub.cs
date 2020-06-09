using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Processing;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub
    {
        #region Inner types

        /// <summary>
        /// Interaction with client side.
        /// </summary>
        /// <remarks>
        /// It's impossible to put it directly into <see cref="JobsHub"/>, because
        /// SignalR disallow overloaded methods, so need to name them differently.
        /// </remarks>
        private class Sender : IResultSender
        {
            /// <summary>
            /// Remote method name to be called on success.
            /// </summary>
            private const string OnComplete = "onComplete";

            /// <summary>
            /// Remote method name to be called on failure.
            /// </summary>
            private const string OnError = "onError";

            /// <summary>
            /// Where to send response.
            /// Notify only the client, who send the job request.
            /// </summary>
            private IClientProxy Destination => _hub.Clients.Caller;
            private readonly Hub _hub;

            public Sender(Hub hub)
            {
                _hub = hub;
            }

            public Task SendSuccessAsync()
            {
                return Destination.SendAsync(OnComplete);
            }

            public Task SendSuccessAsync(object arg0)
            {
                return Destination.SendAsync(OnComplete, arg0);
            }

            public Task SendSuccessAsync(object arg0, object arg1)
            {
                return Destination.SendAsync(OnComplete, arg0, arg1);
            }

            public Task SendSuccessAsync(object arg0, object arg1, object arg2)
            {
                return Destination.SendAsync(OnComplete, arg0, arg1, arg2);
            }

            public Task SendErrorAsync()
            {
                return Destination.SendAsync(OnError);
            }

            public Task SendErrorAsync(object arg0)
            {
                return Destination.SendAsync(OnError, arg0);
            }

            public Task SendErrorAsync(object arg0, object arg1)
            {
                return Destination.SendAsync(OnError, arg0, arg1);
            }

            public Task SendErrorAsync(object arg0, object arg1, object arg2)
            {
                return Destination.SendAsync(OnError, arg0, arg1, arg2);
            }
        }

        #endregion

        private readonly ILogger<JobsHub> _logger;
        private readonly ProjectWork _projectWork;
        private readonly LinkGenerator _linkGenerator;
        private readonly Sender _sender;

        public JobsHub(ILogger<JobsHub> logger, ProjectWork projectWork, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _projectWork = projectWork;
            _linkGenerator = linkGenerator;

            _sender = new Sender(this);
        }

        public Task CreateUpdateJob(string projectId, InventorParameters parameters)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");

            // create job and run it
            var job = new UpdateModelJobItem(_logger, projectId, parameters, _projectWork); // TODO: is it correct to use `Clients.All`?
            return RunJobAsync(job);
        }

        public Task CreateRFAJob(string projectId, string hash)
        {
            _logger.LogInformation($"invoked CreateRFAJob, connectionId : {Context.ConnectionId}");

            // create job and run it
            var job = new RFAJobItem(_logger, projectId, hash, _projectWork, _linkGenerator);
            return RunJobAsync(job);
        }

        public async Task RunJobAsync(JobItemBase job)
        {
            try
            {
                await job.ProcessJobAsync(_sender);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for {job.Id}");
                await _sender.SendErrorAsync(job.Id, e.Message);
            }
        }
    }
}
