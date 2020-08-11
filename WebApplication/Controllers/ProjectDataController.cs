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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    /// <summary>
    /// The controller serves data for default project state. (IOW - no parameter hash is used)
    /// </summary>
    [ApiController]
    public class ProjectDataController : ControllerBase
    {
        private readonly UserResolver _userResolver;
        private readonly ILogger<ProjectDataController> _logger;

        public ProjectDataController(UserResolver userResolver, ILogger<ProjectDataController> logger)
        {
            _userResolver = userResolver;
            _logger = logger;
        }

        [HttpGet("parameters/{projectName}")]
        public async Task<ActionResult> GetParameters(string projectName)
        {
            return await SendLocalFileContent(projectName, LocalName.Parameters);
        }

        [HttpGet("bom/{projectName}")]
        public async Task<ActionResult> GetBOM(string projectName)
        {
            return await SendLocalFileContent(projectName, LocalName.BOM);
        }

        [HttpGet("bom/{projectName}/{hash}")]
        public async Task<ActionResult> GetBOM(string projectName, string hash)
        {
            return await SendLocalFileContent(projectName, LocalName.BOM, hash);
        }

        /// <summary>
        /// Send local file for the project.
        /// </summary>
        private async Task<ActionResult> SendLocalFileContent(string projectName, string fileName, string hash = null, string contentType = "application/json")
        {
            string localFile = await _userResolver.EnsureLocalFile(projectName, fileName);
            return new PhysicalFileResult(localFile, contentType);
        }
    }
}
