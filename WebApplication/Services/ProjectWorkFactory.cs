using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using WebApplication.Processing;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public interface IProjectWorkFactory
    {
        ProjectWork CreateProjectWork(string arrangerUniquePrefix, UserResolver userResolver);
    }

    public class ProjectWorkFactory : IProjectWorkFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProjectWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ProjectWork CreateProjectWork(string arrangerUniquePrefix, UserResolver userResolver)
        {
            return new ProjectWork(
                _serviceProvider.GetRequiredService<ILogger<ProjectWork>>(),
                _serviceProvider.GetRequiredService<FdaClient>(),
                _serviceProvider.GetRequiredService<DtoGenerator>(),
                userResolver,
                _serviceProvider.GetRequiredService<IHttpClientFactory>(),
                arrangerUniquePrefix
            );
        }
    }
}
