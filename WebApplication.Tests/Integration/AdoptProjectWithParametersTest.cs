using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog.Extensions.Logging;
using Shared;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace WebApplication.Tests.Integration
{
    public class AdoptProjectWithParametersTest : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ProjectService _projectService;

        private class DefaultHttpClientFactory : IHttpClientFactory
        {
            public HttpClient CreateClient(string name) => new HttpClient();
        }

        public AdoptProjectWithParametersTest(WebApplicationFactory<WebApplication.Startup> factory, ITestOutputHelper output)
        {
            _output = output;
            XUnitUtils.RedirectConsoleToXUnitOutput(output);

            using var scope = factory.Services.CreateScope();
            _projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
        }

        [Fact(DisplayName = "testAdoptProjectWithParams")]
        public async void testJobWork()
        {
            var guid = Guid.NewGuid().ToString();
            var wrenchSzValue = "\"Small\"";
            var jawOffsetValue = "8 mm";
            var payload = new AdoptProjectWithParametersPayload
            {
                Url = "https://sdra-default-projects.s3-us-west-2.amazonaws.com/Wrench_2021.zip",
                Name = $"TestProject_{guid}",
                TopLevelAssembly = "Wrench.iam",
                Config = new Shared.InventorParameters
                {
                    { "WrenchSz", new Shared.InventorParameter { Value = wrenchSzValue } },
                    { "JawOffset", new Shared.InventorParameter { Value = jawOffsetValue } },
                    { "PartMaterial", new Shared.InventorParameter { Value = "\"Steel\"" } },
                    { "iTrigger0", new Shared.InventorParameter { Value = "2 ul" } }
                }
            };

            // adopt with new values
            var project = await _projectService.AdoptProjectWithParametersAsync(payload);
            // read cached values
            var state = await _projectService.getProjectParameters(project.Id, project.Hash);
            // compare with required values
            Assert.Equal(state.Parameters["WrenchSz"].Value, wrenchSzValue);
            Assert.Equal(state.Parameters["JawOffset"].Value, jawOffsetValue);

            _output.WriteLine($"done, adapted project {project.Id}, hash:{project.Hash}");
        }
    }
}
