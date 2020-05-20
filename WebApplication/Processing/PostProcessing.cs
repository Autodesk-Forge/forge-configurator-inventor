using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;
using WebApplication.Utilities;

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

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        private readonly Lazy<string> _lazyReportDir;

        public PostProcessing(IHttpClientFactory clientFactory, ResourceProvider resourceProvider, ILogger<PostProcessing> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;

            _lazyReportDir = new Lazy<string>(() =>
            {
                var reportDir = Path.Combine(resourceProvider.LocalRootName, "Reports");

                // ensure the directory is exists
                Directory.CreateDirectory(reportDir);

                return reportDir;
            });
        }

        public async Task HandleStatus(WorkItemStatus wiStatus)
        {
            // TODO: make it in background
            if (wiStatus.Status != Status.Success)
            {
                var reportName = $"{DateTime.UtcNow:yyyy-MM-dd_hh-mm-ss}_{wiStatus.Id}.txt";
                string reportFullname = Path.Combine(_lazyReportDir.Value, reportName);

                _logger.LogInformation($"Saving {wiStatus.Id} report to {reportName}");
                try
                {
                    // download report
                    var client = _clientFactory.CreateClient();

                    await using var stream = await client.GetStreamAsync(wiStatus.ReportUrl);
                    await using var fs = new FileStream(reportFullname, FileMode.CreateNew);
                    await stream.CopyToAsync(fs);
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
}
