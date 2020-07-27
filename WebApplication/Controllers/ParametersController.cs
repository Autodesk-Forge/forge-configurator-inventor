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

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("parameters")]
    public class ParametersController : ControllerBase
    {
        private readonly UserResolver _userResolver;
        private readonly ILogger<ParametersController> _logger;

        public ParametersController(UserResolver userResolver, ILogger<ParametersController> logger)
        {
            _userResolver = userResolver;
            _logger = logger;
        }

        [HttpGet("{projectName}")]
        public async Task<InventorParameters> GetParameters(string projectName)
        {
            var projectStorage = await _userResolver.GetProjectStorageAsync(projectName);

            var localNames = projectStorage.GetLocalNames();
            var paramsFile = localNames.Parameters;
            if (! System.IO.File.Exists(paramsFile)) // TODO: unify it someday, not high priority
            {
                _logger.LogInformation($"Restoring missing parameters file for '{projectName}'");

                Directory.CreateDirectory(localNames.BaseDir);

                var bucket = await _userResolver.GetBucketAsync(tryToCreate: false);
                await bucket.DownloadFileAsync(projectStorage.GetOssNames().Parameters, paramsFile);
            }

            return Json.DeserializeFile<InventorParameters>(paramsFile);
        }
    }
}