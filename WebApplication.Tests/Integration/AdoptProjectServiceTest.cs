using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared;
using WebApplication.Definitions;
using WebApplication.Services;
using Xunit;

namespace WebApplication.Tests.Integration
{
    public class AdoptProjectServiceTest : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly WebApplicationFactory<WebApplication.Startup> _factory;

        public AdoptProjectServiceTest(WebApplicationFactory<WebApplication.Startup> factory)
        {
            _factory = factory;
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

        [Theory(Skip = "not a real test, just for development purposes")]
        [ClassData(typeof(AdoptProjectWithParametersDataProvider))]
        public void AdoptProjectWithParameters(AdoptProjectWithParametersPayload payload)
        {
            var service = (AdoptProjectService) _factory.Services.GetService(typeof(AdoptProjectService));

            service.AdoptProjectWithParameters(payload);
        }
    }
}
