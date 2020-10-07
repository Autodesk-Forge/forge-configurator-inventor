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
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("callbacks")]
    public class WorkItemCallbackController : ControllerBase
    {
        private enum ResponseWiType
        {
            UpdateProject,
            AdoptProject
        }

        private readonly ILogger<WorkItemCallbackController> _logger;
        private readonly IHubContext<JobsHub> _hubContext;
        private readonly IPostProcessing _postProcessing;
        private readonly IProjectWorkFactory _projectWorkFactory;
        private readonly UserResolver _userResolver;
        private readonly DtoGenerator _dtoGenerator;

        public WorkItemCallbackController(ILogger<WorkItemCallbackController> logger, IHubContext<JobsHub> hubContext, IPostProcessing postProcessing,
            UserResolver userResolver, IProjectWorkFactory projectWorkFactory, DtoGenerator dtoGenerator)
        {
            _logger = logger;
            _hubContext = hubContext;
            _postProcessing = postProcessing;
            _projectWorkFactory = projectWorkFactory;
            _userResolver = userResolver;
            _dtoGenerator = dtoGenerator;
        }

        [HttpPost("onprojectupdatewicompleted")]
        public IActionResult OnProjectUpdateWiCompleted([FromQuery] string clientId,
            [FromQuery] string hash, [FromQuery] string projectId, [FromQuery] string arrangerPrefix, [FromQuery] string jobId)
        {
            // Run the task asynchronously
            Task.Run(() => ProcessResponse(ResponseWiType.UpdateProject, clientId, projectId, arrangerPrefix, jobId, extraArg1: hash));

            // Response accepted
            return Ok();
        }

        [HttpPost("onadoptwicompleted")]
        public IActionResult OnAdoptWiCompleted([FromQuery] string clientId, [FromQuery] string projectId, [FromQuery] string topLevelAssembly,
            [FromQuery] string arrangerPrefix, [FromQuery] string jobId)
        {
            // Run the task asynchronously
            Task.Run(() => ProcessResponse(ResponseWiType.AdoptProject, clientId, projectId, arrangerPrefix, jobId, extraArg1: topLevelAssembly));

            // Response accepted
            return Ok();
        }

        private async Task<ProcessingResult> ProcessResultFromBody()
        {
            // This serialization requires newtonsoft JSON because our FDA client uses it, otherwise deserializing te enum would fail
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            var bodyStr = await reader.ReadToEndAsync();
            
            var status = JsonConvert.DeserializeObject<WorkItemStatus>(bodyStr);
            status.Stats.TimeFinished = DateTime.UtcNow;

            await _postProcessing.HandleStatus(status);

            ProcessingResult result = new ProcessingResult(status.Stats)
            {
                Success = (status.Status == Status.Success),
                ReportUrl = status.ReportUrl
            };

            return result;
        }

        private async void ProcessResponse(ResponseWiType wiType, string clientId, string projectId, string arrangerPrefix, 
            string jobId, string extraArg1 = null)
        {
            IClientProxy client = null;

            try
            {
                // Grab the SignalR client for response
                client = _hubContext.Clients.Client(clientId);

                // Process the response
                var result = await ProcessResultFromBody();

                // Fork to the right workflow depending on returned WI type
                switch (wiType)
                {
                    case ResponseWiType.UpdateProject:
                        await ProcessUpdateProjectResponseAsync(result, client, extraArg1, projectId, arrangerPrefix, jobId);
                        break;
                    case ResponseWiType.AdoptProject:
                        await ProcessAdoptResponseAsync(result, client, projectId, arrangerPrefix, extraArg1);
                        break;
                    default:
                        throw new Exception($"Cannot process this workflow because its type {wiType} is not implemented");
                }
            }
            catch (FdaProcessingException fpe)
            {
                _logger.LogError(fpe, $"Processing failed for callback jobid: {jobId}");
                await client.SendAsync("onError", new ReportUrlError(jobId, fpe.ReportUrl));
            }
            catch (ProcessingException pe)
            {
                await client.SendAsync("onError", new MessagesError(jobId, pe.Title, pe.Messages));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for callback jobid: {jobId}");

                var message = $"Try to repeat your last action and please report the following message: {e.Message}";
                await client.SendAsync("onError", new MessagesError(jobId, "Internal error", new[] { message }));
            }
        }

        private async Task ProcessAdoptResponseAsync(ProcessingResult result, IClientProxy client, string projectId, 
            string arrangerPrefix, string topLevelAssembly)
        {
            ProjectStorage projectStorage = null;
            FdaStatsDTO stats;

            try
            {
                var projectWork = _projectWorkFactory.CreateProjectWork(arrangerPrefix, _userResolver);
                stats = await projectWork.ProcessAdoptProjectAsync(result, projectId, topLevelAssembly);
                projectStorage = await _userResolver.GetProjectStorageAsync(projectId);
            }
            catch (Exception)
            {
                var bucket = await _userResolver.GetBucketAsync();
                await bucket.DeleteObjectAsync(projectStorage.Project.OSSSourceModel);
                throw; // Throw will take us to ProcessResponse method were the general error handling + client message will occur
            }

            await client.SendAsync("onComplete", _dtoGenerator.ToDTO(projectStorage), stats);
        }

        private async Task ProcessUpdateProjectResponseAsync(ProcessingResult result, IClientProxy client, string hash, string projectId, string arrangerPrefix, string jobId)
        {
            try
            {
                var projectWork = _projectWorkFactory.CreateProjectWork(arrangerPrefix, _userResolver);
                (ProjectStateDTO state, FdaStatsDTO stats) = await projectWork.ProcessUpdateProjectAsync(result, hash, projectId);

                await client.SendAsync("onComplete", state, stats);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing failed for {jobId}");

                var message = $"Try to repeat your last action and please report the following message: {e.Message}";
                await client.SendAsync("OnError", new MessagesError(jobId, "Internal error", new[] { message }));
            }
        }
        
        private async Task ProcessGenerateRfaSatAsync(ProcessingResult result, string clientId, string projectId, string jobId, 
            string hash, string satUrl, string arrangerPrefix)
        {
            var projectWork = _projectWorkFactory.CreateProjectWork(arrangerPrefix, _userResolver);
            await projectWork.ProcessSatGeneratedForRfa(result, clientId, projectId, jobId, hash, satUrl, arrangerPrefix);
        }

        /*
        private async Task ProcessGenerateRfaAsync(ProcessingResult result, )
        {

        }
        */
    }
}
