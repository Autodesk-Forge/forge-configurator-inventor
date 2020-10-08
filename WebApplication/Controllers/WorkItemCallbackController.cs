using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication.Definitions;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("callbacks")]
    public class WorkItemCallbackController : ControllerBase
    {
        private readonly WorkItemCache _wiCache;

        public WorkItemCallbackController(WorkItemCache wiCache)
        {
            _wiCache = wiCache;
        }

        [HttpPost("onwicomplete")]
        public async Task<IActionResult> OnWorkItemComplete()
        {
            // Process the response
            var status = await ProcessResultFromBody();
            var record = _wiCache.TakeRecord(status.Id);
            if (record != null)
                record.Complete(status);

            // Response accepted
            return Ok();
        }

        private async Task<WorkItemStatus> ProcessResultFromBody()
        {
            // This serialization requires newtonsoft JSON because our FDA client uses it, otherwise deserializing te enum would fail
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            var bodyStr = await reader.ReadToEndAsync();

            var status = JsonConvert.DeserializeObject<WorkItemStatus>(bodyStr);
            status.Stats.TimeFinished = DateTime.UtcNow;

            return status;
        }
    }
}
