using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Job;
using WebApplication.Processing;

namespace WebApplication.Controllers
{
    public class JobsHub : Hub
    {
        private readonly ILogger<JobsHub> _logger;
        private readonly ProjectWork _projectWork;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;

        public JobsHub(ILogger<JobsHub> logger, IOptions<DefaultProjectsConfiguration> options, ProjectWork projectWork)
        {
            _logger = logger;
            _projectWork = projectWork;

            _defaultProjectsConfiguration = options.Value;
        }

        public void CreateUpdateJob(string projectId, InventorParameters parameters)
        {
            _logger.LogInformation($"invoked CreateJob, connectionId : {Context.ConnectionId}");
            // create job
            // add to jobprocessor (run in thread inside)
            AddNewJob(new UpdateModelJobItem(projectId, parameters));
        }

        public void CreateRFAJob(string projectId, string hash)
        {
            _logger.LogInformation($"invoked CreateRFAJob, connectionId : {Context.ConnectionId}");
            // create job
            // add to jobprocessor (run in thread inside)
            AddNewJob(new RFAJobItem(projectId, hash));
        }

        public Task AddNewJob(JobItemBase job)
        {
            job.DefaultPrjConfig = _defaultProjectsConfiguration;
            job.PrjWork = _projectWork;

            return job.ProcessJobAsync(_logger, Clients.All);
        }
    }
}
