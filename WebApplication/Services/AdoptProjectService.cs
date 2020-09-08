using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.State;

namespace WebApplication.Services
{
    public class AdoptProjectService
    {
        private readonly ILogger<AdoptProjectService> _logger;
        private readonly ProjectService _projectService;
        private readonly ProjectWork _projectWork;
        private readonly UserResolver _userResolver;

        public AdoptProjectService(ILogger<AdoptProjectService> logger, ProjectService projectService, 
            ProjectWork projectWork, UserResolver userResolver)
        {
            _logger = logger;
            _projectService = projectService;
            _projectWork = projectWork;
            _userResolver = userResolver;
        }

        /// <summary>
        /// https://jira.autodesk.com/browse/INVGEN-45256
        /// </summary>
        /// <param name="payload">project configuration with parameters</param>
        /// <returns>project storage</returns>
        public async Task<ProjectStorage> AdoptProjectWithParametersAsync(AdoptProjectWithParametersPayload payload)
        {
            if (! await DoesProjectAlreadyExistAsync(payload.Name))
            {
                var bucket = await _userResolver.GetBucketAsync();
                var signedUrl = await _projectService.TransferProjectToOssAsync(bucket, payload);
                await _projectWork.AdoptAsync(payload, signedUrl);
            }
            else
            {
                _logger.LogInformation($"project with name {payload.Name} already exists");
            }

            await _projectWork.DoSmartUpdateAsync(payload.Config, payload.Name);

            return await _userResolver.GetProjectStorageAsync(payload.Name);
        }

        private async Task<bool> DoesProjectAlreadyExistAsync(string projectName)
        {
            var existingProjects = await _projectService.GetProjectNamesAsync();

            return existingProjects.Contains(projectName);
        }
    }
}
