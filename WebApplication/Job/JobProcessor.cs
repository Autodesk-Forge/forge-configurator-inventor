using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication.Job
{
    public class JobProcessor
    {
        private readonly IHubContext<JobsHub> _hubContext;

        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly FdaClient _fdaClient;
        private readonly Arranger _arranger;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<JobProcessor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public JobProcessor(IHubContext<JobsHub> hubContext, IOptions<DefaultProjectsConfiguration> optionsAccessor, 
                                FdaClient fdaClient, Arranger arranger, ResourceProvider resourceProvider, ILogger<JobProcessor> logger,
                                IHttpClientFactory httpClientFactory)
                            {
            _hubContext = hubContext;

            _defaultProjectsConfiguration = optionsAccessor.Value;
            _fdaClient = fdaClient;
            _arranger = arranger;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public Task AddNewJob(JobItem job)
        {
            return ProcessJobAsync(job);
        }

        private async Task ProcessJobAsync(JobItem job)
        {
            var projectConfig = _defaultProjectsConfiguration.Projects.FirstOrDefault(cfg => cfg.Name == job.ProjectId);
            if (projectConfig == null)
            {
                throw new ApplicationException($"Attempt to get unknown project ({job.ProjectId})");
            }

            var hash = Crypto.GenerateHashString(job.Data); // TODO: need to ensure JSON is the same each time
            _logger.LogInformation(job.Data);

            var project = _resourceProvider.GetProject(projectConfig.Name);
            var localNames = project.LocalNameProvider(hash);

            // check if the data cached already
            if (Directory.Exists(localNames.SvfDir))
            {
                _logger.LogInformation($"Found cached data corresponded to {hash}");
                await Task.Delay(1);
            }
            else
            {
                // TODO: unify with initialization code
                // TODO: what to do on processing errors?
                var inputDocUrl = await _resourceProvider.CreateSignedUrlAsync(project.OSSSourceModel);
                var parameters = JsonSerializer.Deserialize<InventorParameters>(job.Data);

                var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, projectConfig.TopLevelAssembly, parameters);
                bool status = await _fdaClient.AdoptAsync(adoptionData);
                if (!status)
                {
                    _logger.LogError($"Failed to adopt {project.Name}");
                }
                else
                {
                    // rearrange generated data according to the parameters hash
                    await _arranger.DoAsync(project, projectConfig.TopLevelAssembly);

                    _logger.LogInformation("Cache the project locally");

                    // and now cache the generate stuff locally
                    var projectLocalStorage = new ProjectStorage(project, _resourceProvider);
                    await projectLocalStorage.EnsureLocalAsync(_httpClientFactory.CreateClient());
                }
            }


            // send that we are done to client
            await _hubContext.Clients.All.SendAsync("onComplete", job.Id);
        }
    }
}
