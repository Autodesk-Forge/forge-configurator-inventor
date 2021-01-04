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
    public class ProjectServiceTest : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ProjectService _projectService;

        public ProjectServiceTest(WebApplicationFactory<WebApplication.Startup> factory, ITestOutputHelper output)
        {
            _output = output;
            XUnitUtils.RedirectConsoleToXUnitOutput(output);

            using var scope = factory.Services.CreateScope();
            _projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
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
                                "WrenchSz", new InventorParameter()
                                {
                                    Value = "\"Large\""
                                }
                            },
                            {
                                "JawOffset", new InventorParameter()
                                {
                                    Value = "11 mm"
                                }
                            },
                            {
                                "PartMaterial", new InventorParameter()
                                {
                                    Value = "\"Steel\""
                                }
                            },
                            {
                                "iTrigger0", new InventorParameter()
                                {
                                    Value = "2 ul"
                                }
                            }
                        }
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory(Skip = "not a real test, just for development purposes")]
        [ClassData(typeof(AdoptProjectWithParametersDataProvider))]
        public async void AdoptProjectWithParametersAsync(AdoptProjectWithParametersPayload payload)
        {
            var projectWithParameters = await _projectService.AdoptProjectWithParametersAsync(payload);

            _output.WriteLine($"adopted project with parameters, project label: {projectWithParameters.Label}");
        }

        [Fact(Skip = "not a real test, just for development purposes")]
        public async void DeleteAllProjects()
        {
            await _projectService.DeleteAllProjects();

            _output.WriteLine("all projects deleted");
        }
    }
}
