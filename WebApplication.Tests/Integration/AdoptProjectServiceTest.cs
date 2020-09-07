using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using WebApplication.Definitions;
using WebApplication.Services;
using Xunit;
using Xunit.Abstractions;

namespace WebApplication.Tests.Integration
{
    public class AdoptProjectServiceTest : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly AdoptProjectService _adoptProjectService;

        public AdoptProjectServiceTest(WebApplicationFactory<WebApplication.Startup> factory, ITestOutputHelper output)
        {
            _output = output;
            XUnitUtils.RedirectConsoleToXUnitOutput(output);

            using var scope = factory.Services.CreateScope();
            _adoptProjectService = scope.ServiceProvider.GetRequiredService<AdoptProjectService>();
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
            var projectId = _adoptProjectService.AdoptProjectWithParameters(payload);

            _output.WriteLine($"project created with id {projectId}");
        }
    }
}
