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
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using WebApplication.Middleware;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    // The app is not keeping user session, so it's necessary to pass auth token
    // for authenticated users to resolve the downloaded item correctly.
    // By implementation the token will be appended by client-side to the end of download URL, so
    // the download route should contain optional `token` argument, which will be extracted
    // and applied to the execution context by `RouteTokenPipeline` middleware.
    [ApiController]
    [Route("download")]
    [MiddlewareFilter(typeof(RouteTokenPipeline))]
    public class DownloadController : ControllerBase
    {
        private readonly ILogger<DownloadController> _logger;
        private readonly UserResolver _userResolver;

        public DownloadController(ILogger<DownloadController> logger, UserResolver userResolver)
        {
            _logger = logger;
            _userResolver = userResolver;
        }

        [HttpGet("{projectName}/{hash}/model/{token?}")]
        public Task<RedirectResult> Model(string projectName, string hash, string token = null)
        {
            return RedirectToOssObject(projectName, hash, (ossNames, isAssembly) => ossNames.GetCurrentModel(isAssembly));
        }

        [HttpGet("{projectName}/{hash}/rfa/{token?}")]
        public Task<RedirectResult> RFA(string projectName, string hash, string token = null)
        {
            return RedirectToOssObject(projectName, hash, (ossNames, _)=> ossNames.Rfa);
        }

        [HttpGet("{projectName}/{hash}/bom/{token?}")]
        public async Task<ActionResult> BOM(string projectName, string hash, string token = null)
        {
            string localFileName = await _userResolver.EnsureLocalFile(projectName, LocalName.BOM, hash);
            var bom = Json.DeserializeFile<ExtractedBOM>(localFileName);
            string csv = bom.ToCSV();

            // BOM size is small, so ignore potential performance improvements with direct stream writing
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            return File(stream, "text/csv", "bom.csv");
        }

        // viewer expects PDF extension, so `drawing.pdf` is a piece of URL now (TODO: get rid of it somehow)
        [HttpGet("{projectName}/{hash}/{fileName}/{token?}")]
        public async Task<ActionResult> DrawingPdf(string projectName, string hash, string fileName, string token = null)
        {
            string localFileName = await _userResolver.EnsureLocalFile(projectName, fileName, hash);
            return new PhysicalFileResult(localFileName, "application/pdf") { FileDownloadName = LocalName.DrawingPdf };
        }

        [HttpGet("{projectName}/{hash}/drawing/{token?}")]
        public Task<RedirectResult> Drawing(string projectName, string hash, string token = null)
        {
            return RedirectToOssObject(projectName, hash, (ossNames, _) => ossNames.Drawing);
        }

        private async Task<RedirectResult> RedirectToOssObject(string projectName, string hash, Func<OSSObjectNameProvider, bool, string> nameExtractor)
        {
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName);
            
            var ossNameProvider = projectStorage.GetOssNames(hash);
            string ossObjectName = nameExtractor(ossNameProvider, projectStorage.IsAssembly);

            _logger.LogInformation($"Downloading '{ossObjectName}'");

            var bucket = await _userResolver.GetBucketAsync();
            var url = await bucket.CreateSignedUrlAsync(ossObjectName);

            return Redirect(url);
        }
    }
}
