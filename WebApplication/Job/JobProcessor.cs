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
                foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
                {
                    if (defaultProjectConfig.Name != projectname)
                        continue;

                    var project = _resourceProvider.GetProject(defaultProjectConfig.Name);

                    //await AdoptAsync(httpClient, project, defaultProjectConfig.TopLevelAssembly);
                    // TEMPORARY the similar as we do in AdoptAsync
                    // but with changed parameters ONLY for now

                    var inputDocUrl = await _resourceProvider.CreateSignedUrlAsync(project.OSSSourceModel);
                    var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, defaultProjectConfig.TopLevelAssembly);
                    var status = await _fdaClient.AdoptAsync(adoptionData); // ER: think: it's a business logic, so it might not deal with low-level WI and status
                    if (status.Status != Status.Success)
                    {
                        _logger.LogError($"Failed to adopt {project.Name}");
                    }
                    else
                    {
                        string tempParameters = job.Data;

                        // TEMPORARY generate hash from new parameters here
                        byte[] byteArray = Encoding.ASCII.GetBytes(tempParameters);
                        MemoryStream stream = new MemoryStream(byteArray);
                        string tempHash = Crypto.GenerateStreamHashString(stream);

                        // rearrange generated data according to the parameters hash
                        await _arranger.DoAsync(project, /*TEMPORARY*/tempHash);

                        _logger.LogInformation("Cache the project locally");

                        // and now cache the generate stuff locally
                        var projectLocalStorage = new ProjectStorage(project, _resourceProvider);
                        await projectLocalStorage.EnsureLocalAsync(httpClient, /*TEMPORARY*/tempParameters);
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
