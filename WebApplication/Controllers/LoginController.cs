using System;
using System.Threading.Tasks;
using System.Web;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private static readonly ProfileDTO AnonymousProfile = new ProfileDTO { Name = "Anonymous", AvatarUrl = "logo-xs-white-BG.svg" };

        private readonly ILogger<LoginController> _logger;
        private readonly UserResolver _userResolver;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        public LoginController(ILogger<LoginController> logger, IOptions<ForgeConfiguration> optionsAccessor, UserResolver userResolver)
        {
            _logger = logger;
            _userResolver = userResolver;
            Configuration = optionsAccessor.Value.Validate();
        }

        [HttpGet]
        public RedirectResult Get()
        {
            _logger.LogInformation("Authorize against the Oxygen");

            // prepare redirect URL for Oxygen
            // NOTE: This MUST match the pattern of the callback URL field of the app's registration
            // TODO: workaround which may be removed once application will start to use https
            var scheme = HttpContext.Request.Scheme;
            if (HttpContext.Request.Host.Host == "inventor-config-demo.autodesk.io" ||
                HttpContext.Request.Host.Host == "inventor-config-demo-dev.autodesk.io" )
            {
                scheme = "https";
            }
            var callbackUrl = $"{scheme}{Uri.SchemeDelimiter}{HttpContext.Request.Host}";
            var encodedHost = HttpUtility.UrlEncode(callbackUrl);

            // prepare scope
            var scopes = new[] { "user-profile:read" };
            var fullScope = string.Join("%20", scopes); // it's not necessary now, but kept in case we need it in future

            // build auth url (https://forge.autodesk.com/en/docs/oauth/v2/reference/http/authorize-GET)
            var authUrl = $"{Configuration.AuthenticationAddress.GetLeftPart(System.UriPartial.Authority)}/authentication/v1/authorize?response_type=token&client_id={Configuration.ClientId}&redirect_uri={encodedHost}&scope={fullScope}";
            return Redirect(authUrl);
        }

        [HttpGet("profile")]
        public async Task<ProfileDTO> Profile()
        {
            _logger.LogInformation("Get profile");
            if (_userResolver.IsAuthenticated)
            {
                dynamic profile = await _userResolver.GetProfileAsync();
                return new ProfileDTO { Name = profile.firstName + " " + profile.lastName, AvatarUrl = profile.profileImages.sizeX40 };
            }
            else
            {
                return AnonymousProfile;
            }
        }
    }
}
