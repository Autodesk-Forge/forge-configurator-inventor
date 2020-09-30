using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("callbacks")]
    public class WorkItemCallbackController : ControllerBase
    {
        private readonly ILogger<WorkItemCallbackController> _logger;

        public WorkItemCallbackController(ILogger<WorkItemCallbackController> logger)
        {
            _logger = logger;
        }

        [HttpPost("onwicomplete")]
        public string OnWiComplete()
        {
            return "This endpoint is working";
        }
    }
}
