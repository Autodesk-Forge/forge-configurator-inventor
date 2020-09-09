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
                        //Name = Guid.NewGuid().ToString(),
                        Url = "https://sdra-default-projects.s3-us-west-2.amazonaws.com/Wrench_2021.zip",
                        TopLevelAssembly = "Wrench.iam",
                        Config = new InventorParameters()
                        {
                            {
                                "WrenchSz", new InventorParameter()
                                {
                                    Value = "\"Large\"",
                                    Values = new [] {
                                        "\"Large\"",
                                        "\"Medium\"",
                                        "\"Small\""
                                    },
                                    Unit = "Text"
                                }
                            },
                            {
                                "JawOffset", new InventorParameter()
                                {
                                    Value = "11 mm",
                                    Unit = "mm",
                                    Values = new string [] {}
                                }
                            },
                            {
                                "PartMaterial", new InventorParameter()
                                {
                                    Value = "\"Steel\"",
                                    Values = new [] {
                                        "\"Cast Bronze\"",
                                        "\"Cast Iron\"",
                                        "\"Copper\"",
                                        "\"Gray Iron\"",
                                        "\"Stainless Steel\"",
                                        "\"Steel\""
                                    },
                                    Unit = "Text"
                                }
                            },
                            {
                                "iTrigger0", new InventorParameter()
                                {
                                    Value = "2 ul",
                                    Unit = "ul",
                                    Values = new string [] {}
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
        public void AdoptProjectWithParameters(AdoptProjectWithParametersPayload payload)
        {
            var projectStorage = _projectService.AdoptProjectWithParametersAsync(payload).Result;

            _output.WriteLine($"adopted project with parameters, project name: {projectStorage.Project.Name}");
        }

        [Fact(Skip = "not a real test, just for development purposes")]
        public async void DeleteAllProjects()
        {
            await _projectService.DeleteAllProjects();

            _output.WriteLine("all projects deleted");
        }
    }
}
