using System;
using System.Threading.Tasks;
using System.Web;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IForgeOSS _forge;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        public LoginController(ILogger<LoginController> logger, IOptions<ForgeConfiguration> optionsAccessor, IForgeOSS forge)
        {
            _logger = logger;
            _forge = forge;
            Configuration = optionsAccessor.Value.Validate();
        }

        [HttpGet]
        public RedirectResult Get()
        {
            _logger.LogInformation("Authorize against the Oxygen");

            // prepare redirect URL for Oxygen
            // NOTE: This MUST match the pattern of the callback URL field of the app's registration
            var callbackUrl = $"{HttpContext.Request.Scheme}{Uri.SchemeDelimiter}{HttpContext.Request.Host}";
            var encodedHost = HttpUtility.UrlEncode(callbackUrl);

            // prepare scope
            var scopes = new[] { "data:read", "user-profile:read" };  // TODO: ensure that 'data:read' is necessary
            var fullScope = string.Join("%20", scopes);

            // build auth url (https://forge.autodesk.com/en/docs/oauth/v2/reference/http/authorize-GET)
            var authUrl = $"https://developer.api.autodesk.com/authentication/v1/authorize?response_type=token&client_id={Configuration.ClientId}&redirect_uri={encodedHost}&scope={fullScope}";
            return Redirect(authUrl);
        }

        [HttpGet("profile")]
        public async Task<ProfileDTO> Profile(string token)
        {
            _logger.LogInformation("Get profile");
            var profile = await _forge.GetProfileAsync(token);

            return new ProfileDTO { Name = profile.userName, AvatarUrl = profile.profileImages.sizeX40 };
        }
    }
}
