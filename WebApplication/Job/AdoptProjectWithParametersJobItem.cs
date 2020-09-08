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
using WebApplication.State;
using WebApplication.Definitions;
using WebApplication.Utilities;
using WebApplication.Services;

namespace WebApplication.Job
{
    internal class AdoptProjectWithParametersJobItem : JobItemBase
    {
        private readonly ProjectService _projectService;
        private readonly AdoptProjectWithParametersPayload _payload;
        private readonly DtoGenerator _dtoGenerator;

        public AdoptProjectWithParametersJobItem(ILogger logger, ProjectService projectService, AdoptProjectWithParametersPayload payload, 
            DtoGenerator dtoGenerator)
            : base(logger, null, null)
        {
            _projectService = projectService;
            _payload = payload;
            _dtoGenerator = dtoGenerator;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            using var scope = Logger.BeginScope("Project Adoption ({Id})");

            Logger.LogInformation($"ProcessJob (AdoptProjectWithParameters) {Id} for project {_payload.Name} started.");
            
            try
            {
                ProjectStorage projectStorage = await _projectService.AdoptProjectWithParametersAsync(_payload);

                Logger.LogInformation($"ProcessJob (AdoptProjectWithParameters) {Id} for project {_payload.Name} completed.");
                
                await resultSender.SendSuccessAsync(_dtoGenerator.ToDTO(projectStorage));
            }
            catch (FdaProcessingException fpe)
            {
                await resultSender.SendErrorAsync(Id, fpe.ReportUrl);
            }
        }
    }
}