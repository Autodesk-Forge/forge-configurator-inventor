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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using WebApplication.Services;

namespace WebApplication.Middleware
{
    /// <summary>
    /// Middleware to extract access token from route parameters.
    /// </summary>
    public class RouteTokenHandler
    {
        private readonly RequestDelegate _next;

        public RouteTokenHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ProfileProvider profileProvider, ILogger<RouteTokenHandler> logger)
        {
            string token = (context.GetRouteValue("token") as string)!; // IMPORTANT: parameter name must be in sync with route definition
            if (!string.IsNullOrEmpty(token))
            {
                logger.LogInformation("Extracted token from route");
                profileProvider.Token = token;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    /// <summary>
    /// Special pipeline to extract tokens from route values
    /// </summary>
    public class RouteTokenPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<RouteTokenHandler>();
        }
    }
}
