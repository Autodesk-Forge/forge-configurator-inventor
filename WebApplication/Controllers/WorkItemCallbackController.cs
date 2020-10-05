using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;

namespace WebApplication.Controllers
{
    // This construction is temporary
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SerializableStatus
    {
        Pending = 1,
        Inprogress = 2,
        Cancelled = 3,
        FailedLimitDataSize = 4,
        FailedLimitProcessingTime = 5,
        FailedDownload = 6,
        FailedInstructions = 7,
        FailedUpload = 8,
        Success = 9
    }

    public class Response
    {
        public SerializableStatus Status { get; set; }
        public string Progress { get; set; }
        public string ReportUrl { get; set; }
        public Statistics Stats { get; set; }
        public string Id { get; set; }
    }
    //

    [ApiController]
    [Route("callbacks")]
    public class WorkItemCallbackController : ControllerBase
    {
        private readonly ILogger<WorkItemCallbackController> _logger;
        private readonly IHubContext<JobsHub> _hubContext;
        private readonly IPostProcessing _postProcessing;
        private readonly ProjectWork _projectWork;

        public WorkItemCallbackController(ILogger<WorkItemCallbackController> logger, IHubContext<JobsHub> hubContext, IPostProcessing postProcessing,
            ProjectWork projectWork)
        {
            _logger = logger;
            _hubContext = hubContext;
            _postProcessing = postProcessing;
            _projectWork = projectWork;
        }

        [HttpPost("onwicomplete")]
        public async Task<IActionResult> OnWiComplete([FromBody] Response response, [FromQuery] string clientId, 
            [FromQuery] string hash, [FromQuery] string projectId, [FromQuery] string arrangerPrefix)
        {
            // Process response to SignalR as fire and forget in order to respond to web hook quickly to avoid re-tries
            ProcessResponse(response, clientId, hash, projectId, arrangerPrefix);

            // Rsponse accepted
            return Ok();
        }

        private async void ProcessResponse(Response response, string clientId, string hash, string projectId, string arrangerPrefix)
        {
            try
            {
                WorkItemStatus status = new WorkItemStatus()
                {
                    Status = (Status)response.Status,
                    Progress = response.Progress,
                    ReportUrl = response.ReportUrl,
                    Stats = response.Stats,
                    Id = response.Id
                };

                status.Stats.TimeFinished = DateTime.Now;

                await _postProcessing.HandleStatus(status);

                ProcessingResult result = new ProcessingResult(status.Stats)
                {
                    Success = (status.Status == Status.Success),
                    ReportUrl = status.ReportUrl
                };

                (ProjectStateDTO state, FdaStatsDTO stats) = await _projectWork.ProcessUpdateProject(result, hash, projectId);

                // Grab the SignalR client for response
                var client = _hubContext.Clients.Client(clientId);
                await client.SendAsync("onComplete", state, stats);
            }
            catch (Exception e) { }
        }
    }
}
