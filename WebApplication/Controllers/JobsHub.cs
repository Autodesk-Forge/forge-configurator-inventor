using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Processing;
using WebApplication.State;
using WebApplication.Utilities;

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

            public async Task SendSuccessAsync()
            {
                await Destination.SendAsync(OnComplete);
            }

            public async Task SendSuccessAsync(object arg0)
            {
                await Destination.SendAsync(OnComplete, arg0);
            }

            public async Task SendSuccessAsync(object arg0, object arg1)
            {
                await Destination.SendAsync(OnComplete, arg0, arg1);
            }

            public async Task SendSuccessAsync(object arg0, object arg1, object arg2)
            {
                await Destination.SendAsync(OnComplete, arg0, arg1, arg2);
            }

            public async Task SendErrorAsync()
            {
                await Destination.SendAsync(OnError);
            }

            public async Task SendErrorAsync(object arg0)
            {
                await Destination.SendAsync(OnError, arg0);
            }

            public async Task SendErrorAsync(object arg0, object arg1)
            {
                await Destination.SendAsync(OnError, arg0, arg1);
            }

            public async Task SendErrorAsync(object arg0, object arg1, object arg2)
            {
                await Destination.SendAsync(OnError, arg0, arg1, arg2);
            }
        }

        #endregion

        private readonly ILogger<JobsHub> _logger;
        private readonly ProjectWork _projectWork;
        private readonly LinkGenerator _linkGenerator;
        private readonly UserResolver _userResolver;
        private readonly Sender _sender;
        private readonly Uploads _uploads;
        private readonly DtoGenerator _dtoGenerator;

        public JobsHub(ILogger<JobsHub> logger, ProjectWork projectWork, LinkGenerator linkGenerator, UserResolver userResolver, Uploads uploads, DtoGenerator dtoGenerator)
        {
            _logger = logger;
            _projectWork = projectWork;
            _linkGenerator = linkGenerator;
            _userResolver = userResolver;
            _uploads = uploads;
            _dtoGenerator = dtoGenerator;

            _sender = new Sender(this);
        }

        public async Task CreateUpdateJob(string projectId, InventorParameters parameters, string token)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");

            _userResolver.Token = token;

            // create job and run it
            var job = new UpdateModelJobItem(_logger, projectId, parameters, _projectWork);
            await RunJobAsync(job);
        }

        public async Task CreateRFAJob(string projectId, string hash, string token)
        {
            _logger.LogInformation($"invoked CreateRFAJob, connectionId : {Context.ConnectionId}");

            _userResolver.Token = token;

            // create job and run it
            var job = new RFAJobItem(_logger, projectId, hash, _projectWork, _linkGenerator);
            await RunJobAsync(job);
        }

        public async Task CreateAdoptJob(string packageId, string token)
        {
            _logger.LogInformation($"invoked CreateAdoptJob, connectionId : {Context.ConnectionId}");

            _userResolver.Token = token;

            // create job and run it
            var job = new AdoptJobItem(_logger, packageId, _projectWork, _userResolver, _uploads, _dtoGenerator);
            await RunJobAsync(job);
        }

        public async Task RunJobAsync(JobItemBase job)
        {
            try
            {
                await job.ProcessJobAsync(_sender);
            }
            catch (FdaProcessingException fpe)
            {
                _logger.LogError(fpe, $"Processing failed for {job.Id}");
                await _sender.SendErrorAsync(job.Id, fpe.ReportUrl);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for {job.Id}");
                await _sender.SendErrorAsync(job.Id, e.Message);
            }
        }
    }
}
