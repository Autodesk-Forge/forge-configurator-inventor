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
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Middleware;

namespace WebApplication.Processing
{
    public interface IPostProcessing
    {
        Task HandleStatus(WorkItemStatus wiStatus);
    }

    public class PostProcessing : IPostProcessing
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<PostProcessing> _logger;
        private readonly SaveReport _saveReport;

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        private readonly Lazy<string> _lazyReportDir;


        public PostProcessing(IHttpClientFactory clientFactory, ILogger<PostProcessing> logger, LocalCache localCache, IOptions<ProcessingOptions> options)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _saveReport = options.Value.SaveReport;

            _lazyReportDir = new Lazy<string>(() =>
            {
                var reportDir = Path.Combine(localCache.LocalRootName, "Reports");

                // ensure the directory is exists
                Directory.CreateDirectory(reportDir);

                return reportDir;
            });
        }

        public async Task HandleStatus(WorkItemStatus wiStatus)
        {
            if (_saveReport == SaveReport.Off) return;
            if (_saveReport == SaveReport.ErrorsOnly && wiStatus.Status == Status.Success) return;

            // TODO: make it in background
            var reportName = $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}_{wiStatus.Id}.txt";
            string reportFullname = Path.Combine(_lazyReportDir.Value, reportName);

            _logger.LogInformation($"Saving {wiStatus.Id} report to {reportName}");
            try
            {
                // download and save report
                HttpClient client = _clientFactory.CreateClient();

                await using var reportStream = await client.GetStreamAsync(wiStatus.ReportUrl);
                await using var reportFileStream = new FileStream(reportFullname, FileMode.CreateNew);
                await reportStream.CopyToAsync(reportFileStream);

                // save statistics
                if (wiStatus.Stats != null)
                {
                    var statsFullName = Path.ChangeExtension(reportFullname, ".stats.json");

                    await using var statsFileStream = new FileStream(statsFullName, FileMode.CreateNew);
                    await using var jsonWriter = new Utf8JsonWriter(statsFileStream);
                    JsonSerializer.Serialize(jsonWriter, wiStatus.Stats);
                }
            }
            catch (Exception e)
            {
                // downloading report should not stop site functionality,
                // so write log error message and swallow the exception
                _logger.LogError(e, $"Failed to download report for {wiStatus.Id}");
            }
        }
    }
}
