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

using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication.Controllers
{
    [Route("[controller]")]
    public class ClearSelfController : ControllerBase
    {
        private readonly ILogger<DataMigrationController> _logger;
        Initializer _initializer;
        IConfiguration _configuration;

        public ClearSelfController(ILogger<DataMigrationController> logger, Initializer initializer, IConfiguration configuration)
        {
            _logger = logger;
            _initializer = initializer;
            _configuration = configuration;
        }

        [HttpGet("")]
        public string Clear()
        {
            if (_configuration.GetValue<bool>("allowCleanSelf"))
            {
               _logger.LogInformation("Clearing the data...");
               _initializer.ClearAsync(true).Wait();
               _logger.LogInformation("Data deleted.");
               return "{ \"Cleared\": \"true\" }";
            }
            else
               _logger.LogInformation("Self clean not allowed.");

            return "{ \"Cleared\": \"false\" }";
        }
    }
}
