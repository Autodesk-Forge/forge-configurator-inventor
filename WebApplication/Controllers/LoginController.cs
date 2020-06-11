using System;
using System.Net;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }
        public LoginController(IOptions<ForgeConfiguration> optionsAccessor)
        {
            Configuration = optionsAccessor.Value.Validate();
        }

        [HttpGet]
        public RedirectResult Get(string projectName, string hash)
        {
            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var encodedHost = WebUtility.UrlEncode(host);
            var url = $"https://developer.api.autodesk.com/authentication/v1/authorize?response_type=token&client_id={Configuration.ClientId}&redirect_uri={encodedHost}&scope=data:read";
            return Redirect(url);
        }

        
    }
}
