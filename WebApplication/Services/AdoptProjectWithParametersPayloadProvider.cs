using System;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;

namespace WebApplication.Services
{
    public class AdoptProjectWithParametersPayloadProvider
    {
        private readonly ILogger<AdoptProjectWithParametersPayloadProvider> _logger;

        public AdoptProjectWithParametersPayloadProvider(ILogger<AdoptProjectWithParametersPayloadProvider> logger)
        {
            _logger = logger;
        }

        public AdoptProjectWithParametersPayload GetParameters(string jsonFileUrl)
        {
            _logger.LogInformation($"downloading parameters from {jsonFileUrl}");

            var bytes = new WebClient().DownloadData(jsonFileUrl);
            string jsonAsString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            _logger.LogDebug($"parsing parameters from {jsonAsString}");

            return JsonSerializer.Deserialize<AdoptProjectWithParametersPayload>(jsonAsString);
        }
    }
}
