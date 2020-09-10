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

        public AdoptProjectWithParametersPayloadProviderTest(ITestOutputHelper output)
        {
            _output = output;
            XUnitUtils.RedirectConsoleToXUnitOutput(output);

            ILogger<AdoptProjectWithParametersPayloadProvider> logger = new SerilogLoggerFactory().CreateLogger<AdoptProjectWithParametersPayloadProvider>();
            _adoptProjectWithParametersPayloadProvider = new AdoptProjectWithParametersPayloadProvider(logger);
        }

        [Fact(Skip = "just for development purposes")]
        public void GetParameters()
        {
            var payload = _adoptProjectWithParametersPayloadProvider.GetParameters("http://localhost:5080/fileprovider/fileContent");

            _output.WriteLine($"{payload.Name}");
        }
    }
}
