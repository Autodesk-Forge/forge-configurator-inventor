using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("callbacks")]
    public class WorkItemCallbackController : ControllerBase
    {
        private readonly ILogger<WorkItemCallbackController> _logger;
        private readonly IHubContext<JobsHub> _hubContext;
        private readonly IPostProcessing _postProcessing;
        private readonly IProjectWorkFactory _projectWorkFactory;
        private readonly UserResolver _userResolver;

        public WorkItemCallbackController(ILogger<WorkItemCallbackController> logger, IHubContext<JobsHub> hubContext, IPostProcessing postProcessing,
            UserResolver userResolver, IProjectWorkFactory projectWorkFactory)
        {
            _logger = logger;
            _hubContext = hubContext;
            _postProcessing = postProcessing;
            _projectWorkFactory = projectWorkFactory;
            _userResolver = userResolver;
        }

        [HttpPost("onwicomplete")]
        public async Task<IActionResult> OnWiComplete([FromQuery] string clientId,
            [FromQuery] string hash, [FromQuery] string projectId, [FromQuery] string arrangerPrefix, [FromQuery] string jobId)
        {
            // Process response to SignalR as fire and forget in order to respond to web hook quickly to avoid re-tries
            ProcessResponse(clientId, hash, projectId, arrangerPrefix, jobId);

            // Rsponse accepted
            return Ok();
        }

        private async void ProcessResponse(string clientId, string hash, string projectId, string arrangerPrefix, string jobId)
        {
            // Grab the SignalR client for response
            var client = _hubContext.Clients.Client(clientId);

            try
            {
                WorkItemStatus status;
                // This serialization requires newtonsoft JSON because our FDA client uses it, otherwise deserializing te enum would fail
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var bodyStr = await reader.ReadToEndAsync();
                    status = JsonConvert.DeserializeObject<WorkItemStatus>(bodyStr);
                }

                status.Stats.TimeFinished = DateTime.Now;

                await _postProcessing.HandleStatus(status);

                ProcessingResult result = new ProcessingResult(status.Stats)
                {
                    Success = (status.Status == Status.Success),
                    ReportUrl = status.ReportUrl
                };

                var projectWork = _projectWorkFactory.CreateProjectWork(arrangerPrefix, _userResolver);
                (ProjectStateDTO state, FdaStatsDTO stats) = await projectWork.ProcessUpdateProject(result, hash, projectId);

                await client.SendAsync("onComplete", state, stats);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for {jobId}");

                var message = $"Try to repeat your last action and please report the following message: {e.Message}";
                await client.SendAsync("OnError", new MessagesError(jobId, "Internal error", new[] { message }));
            }
        }
    }
}
