using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using WebApplication.Services;
using Xunit;
using Xunit.Abstractions;

namespace WebApplication.Tests.Integration
{
    public class AdoptProjectWithParametersPayloadProviderTest
    {
        private readonly ITestOutputHelper _output;
        private readonly AdoptProjectWithParametersPayloadProvider _adoptProjectWithParametersPayloadProvider;
        
        private class DefaultHttpClientFactory : IHttpClientFactory
        {
            public HttpClient CreateClient(string name) => new HttpClient();
        }

        public AdoptProjectWithParametersPayloadProviderTest(ITestOutputHelper output)
        {
            _output = output;
            XUnitUtils.RedirectConsoleToXUnitOutput(output);

            ILogger<AdoptProjectWithParametersPayloadProvider> logger = new SerilogLoggerFactory().CreateLogger<AdoptProjectWithParametersPayloadProvider>();
            _adoptProjectWithParametersPayloadProvider = new AdoptProjectWithParametersPayloadProvider(logger, new DefaultHttpClientFactory());
        }

        [Fact(Skip = "just for development purposes")]
        public async void GetParametersAsync()
        {
            var payload = await _adoptProjectWithParametersPayloadProvider.GetParametersAsync("http://localhost:5080/fileprovider/fileContent");

            _output.WriteLine($"{payload.Name}");
        }
    }
}
