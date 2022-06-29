/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System.Web;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using webapplication.Definitions;
using webapplication.Services;
using webapplication.Utilities;

namespace webapplication.Controllers;

[ApiController]
[Route("login")]
public class LoginController : ControllerBase
{
    private static readonly ProfileDTO AnonymousProfile = new() { Name = "Anonymous", AvatarUrl = "logo-xs-white-BG.svg" };

    private readonly ILogger<LoginController> _logger;
    private readonly ProfileProvider _profileProvider;
    private readonly InviteOnlyModeConfiguration _inviteOnlyModeConfig;

    /// <summary>
    /// Forge configuration.
    /// </summary>
    public ForgeConfiguration Configuration { get; }

    public LoginController(ILogger<LoginController> logger, IOptions<ForgeConfiguration> optionsAccessor, ProfileProvider profileProvider, IOptions<InviteOnlyModeConfiguration> inviteOnlyModeOptionsAccessor)
    {
        _logger = logger;
        _profileProvider = profileProvider;
        Configuration = optionsAccessor.Value.Validate();
        _inviteOnlyModeConfig = inviteOnlyModeOptionsAccessor.Value;
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
            HttpContext.Request.Host.Host == "inventor-config-demo-dev.autodesk.io")
        {
            scheme = "https";
        }
        var callbackUrl = $"{scheme}{Uri.SchemeDelimiter}{HttpContext.Request.Host}";
        var encodedHost = HttpUtility.UrlEncode(callbackUrl);

        // prepare scope
        var scopes = new[] { "user-profile:read" };
        var fullScope = string.Join("%20", scopes); // it's not necessary now, but kept in case we need it in future

        // build auth url (https://forge.autodesk.com/en/docs/oauth/v2/reference/http/authorize-GET)
        string baseUrl = Configuration.AuthenticationAddress.GetLeftPart(System.UriPartial.Authority);
        var authUrl = $"{baseUrl}/authentication/v1/authorize?response_type=token&client_id={Configuration.ClientId}&redirect_uri={encodedHost}&scope={fullScope}";
        return Redirect(authUrl);
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDTO>> Profile()
    {
        _logger.LogInformation("Get profile");
        if (_profileProvider.IsAuthenticated)
        {
            dynamic profile = await _profileProvider.GetProfileAsync();
            if (_inviteOnlyModeConfig.Enabled)
            {
                var inviteOnlyChecker = new InviteOnlyChecker(_inviteOnlyModeConfig);
                if (!profile.emailVerified || !inviteOnlyChecker.IsInvited(profile.emailId))
                {
                    return StatusCode(403);
                }
            }
            return new ProfileDTO { Name = profile.firstName + " " + profile.lastName, AvatarUrl = profile.profileImages.sizeX40 };
        }
        else
        {
            return AnonymousProfile;
        }
    }
}
