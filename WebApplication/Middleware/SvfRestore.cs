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

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using webapplication.State;

namespace webapplication.Middleware
{
    /// <summary>
    /// Middleware to restore missing SVFs in local cache
    /// </summary>
    public class SvfRestore
    {
        private readonly RequestDelegate _next;

        public SvfRestore(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserResolver userResolver, ILogger<SvfRestore> logger)
        {
            var httpRequest = context.Request;

            while (true)
            {
                // the expected path is like "/data/4EC4EC1C4C0082AB28582C8A50FFC2BF33E42356/Wrench/0B81352BCE7C9CEB8C8EAA7297A8AB64274C75A5/SVF/bubble.json"
                // 0 - 'root' for static files (data)
                // 1 - User dir (4EC4EC1C4C0082AB28582C8A50FFC2BF33E42356
                // 2 - Project ID (Wrench)
                // 3 - Parameters hash (0B81352BCE7C9CEB8C8EAA7297A8AB64274C75A5)
                // 4 - Subdir for SVF structure (SVF)
                // 5 - Manifest file for SVF (bubble.json)
                string[] pieces = httpRequest.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length != 6) break;

                string projectName = pieces[2];
                string hash = pieces[3];

                var projectStorage = await userResolver.GetProjectStorageAsync(projectName);

                // check if SVF dir already exists
                var svfDir = projectStorage.GetLocalNames(hash).SvfDir;
                if (Directory.Exists(svfDir)) break;

                logger.LogInformation($"Restoring '{hash}' for '{projectName}'");
                
                // download and extract SVF
                var bucket = await userResolver.GetBucketAsync();
                await projectStorage.EnsureSvfAsync(bucket, hash);

                break;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
