using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Controllers;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication.Job
{
    public interface IJobProcessor
    {
        void AddNewJob(JobItem job);
    }

    public class JobProcessor : IJobProcessor
    {
        List<Task> _jobs = new List<Task>();
        
        private readonly IHubContext<UpdateJobHub> _hubContext;
        private readonly DesignAutomationClient _client;

        // vvvvvv  temporary  vvvvvvv
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly FdaClient _fdaClient;
        private readonly Arranger _arranger;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<JobProcessor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public JobProcessor(IHubContext<UpdateJobHub> hubContext, DesignAutomationClient client,
            // TEMPORARY
            IOptions<DefaultProjectsConfiguration> optionsAccessor, FdaClient fdaClient, Arranger arranger,
            ResourceProvider resourceProvider, ILogger<JobProcessor> logger,
            IHttpClientFactory httpClientFactory
            // TEMPORARY
            )
        {
            _hubContext = hubContext;
            _client = client;

            // TEMPORARY
            _defaultProjectsConfiguration = optionsAccessor.Value;
            _fdaClient = fdaClient;
            _arranger = arranger;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            // TEMPORARY
        }

        public void AddNewJob(JobItem job)
        {
            _jobs.Add(ProcessJob(job));
        }

        private async Task ProcessJob(JobItem job)
        {
            // TEMPORARY
            // do the similar work like we have in initializer UNTIL we have finished
            // the final activity which will do the final job for us
            // parameters are HACKED locally
            {
                var httpClient = _httpClientFactory.CreateClient();

                string projectname = job.ProjectId;
                foreach (DefaultProjectConfiguration projectConfig in _defaultProjectsConfiguration.Projects)
                {
                    if (projectConfig.Name != projectname)
                        continue;

                    var project = _resourceProvider.GetProject(projectConfig.Name);

                    var inputDocUrl = await _resourceProvider.CreateSignedUrlAsync(project.OSSSourceModel);
                    var parameters = JsonSerializer.Deserialize<InventorParameters>(job.Data);
                    //var parameters = new InventorParameters
                    //{
                    //    { "WrenchSz", new InventorParameter { Value = "\"Large\"" }},
                    //    { "PartMaterial", new InventorParameter { Value = "\"Cast Bronze\"" }}
                    //};


                    var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, projectConfig.TopLevelAssembly, parameters);
                    bool status = await _fdaClient.AdoptAsync(adoptionData);
                    if (! status)
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
                        await projectLocalStorage.EnsureLocalAsync(httpClient);
                    }

                    break;
                }
            }
            // TEMPORARY

            // send that we are done to client
            await _hubContext.Clients.All.SendAsync("onComplete", job.Id);
        }
    }
}
