using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using webapplication.Definitions;

namespace webapplication.Services
{
    public class AdoptProjectWithParametersPayloadProvider
    {
        private readonly ILogger<AdoptProjectWithParametersPayloadProvider> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public AdoptProjectWithParametersPayloadProvider(ILogger<AdoptProjectWithParametersPayloadProvider> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<AdoptProjectWithParametersPayload> GetParametersAsync(string? jsonFileUrl)
        {
            _logger.LogInformation($"downloading parameters from {jsonFileUrl}");

            var httpClient = _clientFactory.CreateClient();
            await using var httpStream = await httpClient.GetStreamAsync(jsonFileUrl);

            StreamReader reader = new StreamReader(httpStream);
            var jsonAsString = await reader.ReadToEndAsync();

            _logger.LogDebug($"parsing parameters from {jsonAsString}");

            return JsonSerializer.Deserialize<AdoptProjectWithParametersPayload>(jsonAsString)!;
        }
    }
}
