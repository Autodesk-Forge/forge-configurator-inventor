using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using WebApplication.Definitions;
using WebApplication.Services;
using Xunit;

namespace WebApplication.Tests.Integration
{
    public class AdoptProjectServiceTest : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly ILogger<AdoptProjectServiceTest> _logger;
        private readonly WebApplicationFactory<WebApplication.Startup> _factory;

        public AdoptProjectServiceTest(WebApplicationFactory<WebApplication.Startup> factory)
        {
            _factory = factory;

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<AdoptProjectServiceTest>();
        }

        public class AdoptProjectWithParametersDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new AdoptProjectWithParametersPayload()
                    {
                        Name = "TestWrench",
                        Url = "https://sdra-default-projects.s3-us-west-2.amazonaws.com/Wrench_2021.zip",
                        TopLevelAssembly = "Wrench.iam",
                        Config = new InventorParameters()
                        {
                            {
                                "Some Param", new InventorParameter()
                                {
                                    Label = "Some Label"
                                }
                            }
                        }
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]//(Skip = "not a real test, just for development purposes")]
        [ClassData(typeof(AdoptProjectWithParametersDataProvider))]
        public void AdoptProjectWithParameters(AdoptProjectWithParametersPayload payload)
        {
            //TODO: move to setUp method
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<AdoptProjectService>();

            var projectId = service.AdoptProjectWithParameters(payload);

            _logger.LogInformation($"project created with id {projectId}");
        }
    }
}
