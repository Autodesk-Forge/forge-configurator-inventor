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
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.State;
using WebApplication.Processing;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("[controller]")]
    public class DataMigrationController : ControllerBase
    {
        private readonly ILogger<DataMigrationController> _logger;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly UserResolver _userResolver;
        private readonly ProjectWork _projectWork;
        private readonly OssBucket _bucket;

        public DataMigrationController(ILogger<DataMigrationController> logger, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            ProjectWork projectWork, UserResolver userResolver)
        {
            _logger = logger;
            _userResolver = userResolver;
            _bucket = _userResolver.AnonymousBucket;
            _projectWork = projectWork;
            _defaultProjectsConfiguration = optionsAccessor.Value;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string returnValue = "";
            foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
            {
                returnValue += "Project " + defaultProjectConfig.Name + " is being adopted\n";
                var projectUrl = defaultProjectConfig.Url;
                var project = await _userResolver.GetProjectAsync(defaultProjectConfig.Name);

                string signedUrl = await _bucket.CreateSignedUrlAsync(project.OSSSourceModel, ObjectAccess.ReadWrite);

                await _projectWork.AdoptAsync(defaultProjectConfig, signedUrl);

                returnValue += "Project " + defaultProjectConfig.Name + " was adopted\n";
            }

            return returnValue;
        }
    }
}
