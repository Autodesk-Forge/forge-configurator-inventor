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
using WebApplication.Definitions;
using System.Threading.Tasks;
using Shared;
using WebApplication.Processing;

namespace WebApplication.Job
{
    public class UpdateModelJobItem : JobItemBase
    {
        public InventorParameters Parameters { get; }

        public UpdateModelJobItem(ILogger logger, string projectId, InventorParameters parameters, ProjectWork projectWork)
            : base(logger, projectId, projectWork)
        {
            Parameters = parameters;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            using var scope = Logger.BeginScope("Update Model ({Id})");

            Logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} started.");

            ProjectStateDTO updatedState = await ProjectWork.DoSmartUpdateAsync(Parameters, ProjectId);

            Logger.LogInformation($"ProcessJob (Update) {Id} for project {ProjectId} completed.");

            // send that we are done to client
            await resultSender.SendSuccessAsync(updatedState);
        }
    }
}
