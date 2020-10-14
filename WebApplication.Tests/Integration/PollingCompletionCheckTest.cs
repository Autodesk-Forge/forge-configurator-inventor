using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;
using Xunit;

namespace WebApplication.Tests.Integration
{
    public class PollingCompletionCheckTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ProjectWork _projectWork;
        private readonly Publisher _publisher;
        private readonly ProjectService _projectService;
        private readonly OssBucket _anonymousBucket;

        public PollingCompletionCheckTest(WebApplicationFactory<Startup> factory)
        {
            using var scope = factory.Services.CreateScope();
            _projectWork = scope.ServiceProvider.GetRequiredService<ProjectWork>();
            _publisher = scope.ServiceProvider.GetRequiredService<Publisher>();
            _projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
            _anonymousBucket = scope.ServiceProvider.GetRequiredService<UserResolver>().AnonymousBucket;
        }

        private class AdoptProjectWithParametersDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var projectConfiguration = new DefaultProjectConfiguration()
                {
                    Name = "TestWrench",
                    Url = "https://sdra-default-projects.s3-us-west-2.amazonaws.com/Wrench_2021.zip",
                    TopLevelAssembly = "Wrench.iam"
                };
                
                yield return new object[]
                {
                    projectConfiguration,
                    CompletionCheck.Polling
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(AdoptProjectWithParametersDataProvider))]
        public async void AdoptProjectWithParametersAsync(DefaultProjectConfiguration payload, CompletionCheck check)
        {
            //given
            _publisher.CompletionCheck = check;

            //when
            var signedUrl = await _projectService.TransferProjectToOssAsync(_anonymousBucket, payload);
            await _projectWork.AdoptAsync(payload, signedUrl);
            var projectNames = await _projectService.GetProjectNamesAsync();

            //then
            Assert.Contains(projectNames, str => str.Equals(payload.Name));

            //tidy up
            await _projectService.DeleteProjects(new List<string> {payload.Name}, _anonymousBucket);
        }
    }
}