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
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using webapplication.Services;
using webapplication.State;

namespace webapplication.Middleware
{
    /// <summary>
    /// Middleware to extract access token from HTTP headers.
    /// </summary>
    public class HeaderTokenHandler
    {
        private const string BearerPrefix = "Bearer ";

        private readonly RequestDelegate _next;

        public HeaderTokenHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ProfileProvider profileProvider)
        {
            while (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
            {
                var headerValue = values[0];
                if (headerValue.Length <= BearerPrefix.Length) break;
                if (! headerValue.StartsWith(BearerPrefix)) break;

                string token = headerValue.Substring(BearerPrefix.Length);
                if (string.IsNullOrEmpty(token)) break;

                profileProvider.Token = token;
                break;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}