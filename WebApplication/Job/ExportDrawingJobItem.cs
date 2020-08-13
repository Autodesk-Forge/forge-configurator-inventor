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

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using WebApplication.Processing;
using System.Threading;

namespace WebApplication.Job
{
    internal class ExportDrawingJobItem : JobItemBase
    {
        private readonly string _hash;
        private readonly LinkGenerator _linkGenerator;

        public ExportDrawingJobItem(ILogger logger, string projectId, string hash, ProjectWork projectWork, LinkGenerator linkGenerator)
            : base(logger, projectId, projectWork)
        {
            _hash = hash;
            _linkGenerator = linkGenerator;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            using var scope = Logger.BeginScope("Export Drawing ({Id})");

            Logger.LogInformation($"ProcessJob (ExportDrawing) {Id} for project {ProjectId} started.");

            bool generated = await ProjectWork.ExportDrawingViewablesAsync(ProjectId, _hash);

            Logger.LogInformation($"ProcessJob (ExportDrawing) {Id} for project {ProjectId} completed.");

            string url = null;
            if (generated)
            {
                url = _linkGenerator.GetPathByAction(controller: "Download",
                                                                action: "DrawingViewables",
                                                                values: new { projectName = ProjectId, fileName = "drawing.pdf", hash = _hash });

                // when local url starts with slash, it does not work, because it is doubled in url
                url = url.IndexOf("/") == 0 ? url.Substring(1) : url;
            }

            await resultSender.SendSuccessAsync(url);
        }
    }
}