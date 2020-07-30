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
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Autodesk.Forge.Model;
using Shared;
using WebApplication.Definitions;
using WebApplication.State;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.Utilities;
using WebApplication.Middleware;

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
        private readonly IForgeOSS _forgeOSS;
        private readonly LocalCache _localCache;

        public DataMigrationController(ILogger<DataMigrationController> logger, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            ProjectWork projectWork, UserResolver userResolver, IForgeOSS forgeOSS, LocalCache localCache)
        {
            _logger = logger;
            _userResolver = userResolver;
            _bucket = _userResolver.AnonymousBucket;
            _projectWork = projectWork;
            _defaultProjectsConfiguration = optionsAccessor.Value;
            _forgeOSS = forgeOSS;
            _localCache = localCache;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string returnValue = "";
            List<ObjectDetails> ossFiles = await _forgeOSS.GetBucketObjectsAsync(_bucket.BucketKey, "cache/");
            foreach (ObjectDetails file in ossFiles)
            {
                string[] fileParts = file.ObjectKey.Split('/');
                string project = fileParts[1];
                string hash = fileParts[2];
                string fileName = fileParts[3];
                if (fileName == "parameters.json")
                {
                    returnValue += "Project " + project + " (" + hash + ") is being updated\n";
                    string paramsFile = Path.Combine(_localCache.LocalRootName, "params.json");
                    await _bucket.DownloadFileAsync(file.ObjectKey, paramsFile);
                    InventorParameters inventorParameters = Json.DeserializeFile<InventorParameters>(paramsFile);
                    await _projectWork.DoSmartUpdateAsync(inventorParameters, project, true);
                    returnValue += "Project " + project + " (" + hash + ") was updated\n";
                }
            }

            return returnValue;
        }

        [HttpGet("Adopt")]
        public async Task<string> AdoptDefaultOnly()
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
