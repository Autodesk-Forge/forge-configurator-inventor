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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace WebApplication.Middleware
{
    /// <summary>
    /// For performance reason - some important generated files are cached locally,
    /// this class encapsulate related logic and allows access to it as to static files.
    /// </summary>
    public class LocalCache
    {
        private const string LocalCacheDir = "LocalCache";
        public const string VirtualCacheDir = "/data";

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir);

        public LocalCache()
        {
            // make sure that root directory exists
            Directory.CreateDirectory(LocalRootName);
        }

        /// <summary>
        /// Get URL pointing for the data file.
        /// </summary>
        /// <param name="localFileName">Full filename. Must be under "local cache root"</param>
        public string ToDataUrl(string localFileName)
        {
            if (!localFileName.StartsWith(LocalRootName, StringComparison.InvariantCultureIgnoreCase))
                throw new ApplicationException("Attempt to generate URL for non-data file");

            string relativeName = localFileName.Substring(LocalRootName.Length);
            return VirtualCacheDir + relativeName.Replace('\\', '/');
        }

        /// <summary>
        /// Expose local cache dir as 'data' virtual dir to serve locally cached OSS files
        /// </summary>
        public void Serve(IApplicationBuilder app)
        {
            // 'bubble.json' is a top-level manifest file for SVF structure. Detect requests to it,
            // and (if necessary) restore SVF to local cache from OSS
            app.UseWhen(context => context.Request.Path.Value.StartsWith(VirtualCacheDir) &&
                                   context.Request.Path.Value.EndsWith("bubble.json"),
                appBuilder =>
                {
                    appBuilder.UseMiddleware<HeaderTokenHandler>();
                    appBuilder.UseMiddleware<SvfRestore>();
                });

            // serve Local Cache dir as static files in '/data' virtual dir
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(LocalRootName),
                RequestPath = new PathString(VirtualCacheDir),
                ServeUnknownFileTypes = true
            });
        }
    }
}
