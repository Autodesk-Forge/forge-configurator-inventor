using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("parameters")]
    public class ParametersController : ControllerBase
    {
        private readonly ResourceProvider _resourceProvider;
        // SignalR Hub
        private IHubContext<signalRHub> _hubContext;

        public ParametersController(ResourceProvider resourceProvider, IHubContext<signalRHub> hubContext)
        {
            _resourceProvider = resourceProvider;
            _hubContext = hubContext;
        }

        [HttpGet("{projectName}")]
        public InventorParameters GetParameters(string projectName)
        {
            var projectStorage = _resourceProvider.GetProjectStorage(projectName);
            var paramsFile = projectStorage.GetLocalNames().Parameters;
            return Json.DeserializeFile<InventorParameters>(paramsFile);
        }

        [HttpPost("{projectName}")]
        public async void UpdateModelWithParameters(string connectionId, string projectName, [FromBody]dynamic body)
        {
            Guid jobId = Guid.NewGuid();
            string bodyStr = (string)body.ToString();
            JObject bodyJson = JObject.Parse(bodyStr);
            int sleep = bodyJson["sleep"].Value<int>();

            await _hubContext.Clients.Client(connectionId).SendAsync("onStarted", jobId.ToString(), bodyStr);

            // wait interval we recieved now
            Thread.Sleep(sleep);

            // done
            await _hubContext.Clients.Client(connectionId).SendAsync("onComplete", jobId.ToString());
        }
    }
}