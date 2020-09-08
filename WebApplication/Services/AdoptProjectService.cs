using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public class AdoptProjectService
    {
        private readonly ILogger<AdoptProjectService> _logger;
        private readonly ProjectService _projectService;
        private readonly ProjectWork _projectWork;
        private readonly Uploads _uploads;
        private readonly UserResolver _userResolver;

        public AdoptProjectService(ILogger<AdoptProjectService> logger, ProjectService projectService, 
            Uploads uploads, ProjectWork projectWork, UserResolver userResolver)
        {
            _logger = logger;
            _projectService = projectService;
            _uploads = uploads;
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
                var packageId = await CreateProjectAsync(payload);
                await AdoptProjectAsync(packageId);
            }
            else
            {
                _logger.LogInformation($"project with name {payload.Name} already exists");
            }

            await UpdateParamsAsync(payload);

            return await _userResolver.GetProjectStorageAsync(payload.Name);
        }

        private async Task<bool> DoesProjectAlreadyExistAsync(string projectName)
        {
            var existingProjects = await _projectService.GetProjectNamesAsync();

            return existingProjects.Contains(projectName);
        }

        /// <summary>
        /// Downloads project ZIP file and delegates to ProjectService.CreateProject
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private async Task<string> CreateProjectAsync(DefaultProjectConfiguration configuration)
        {
            _logger.LogInformation($"start of CreateProjectAsync, projectName {configuration.Name}");

            using var localFile = new TempFile();
            var localFileName = localFile.Name;
            using (var client = new WebClient())
            {
                _logger.LogInformation($"downloading project from {configuration.Url} to {localFileName}");
                client.DownloadFile(configuration.Url, localFileName);
            }

            using FileStream stream = File.OpenRead(localFileName);

            _logger.LogInformation($"creating project {configuration.Name}");

            return await _projectService.CreateProject(new NewProjectModel()
            {
                package = new FormFile(stream, 0, stream.Length, null, configuration.Name)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                },
                root = configuration.TopLevelAssembly
            });
        }

        /// <summary>
        /// Gets the project data and delegates to ProjectService.AdoptProject
        /// </summary>
        /// <returns></returns>
        private Task<ProjectStorage> AdoptProjectAsync(string packageId)
        {
            _logger.LogInformation($"start of AdoptProjectAsync, packageId {packageId}");

            (ProjectInfo projectInfo, string fileName) = _uploads.GetUploadData(packageId);
            _uploads.ClearUploadData(packageId);

            var id = Guid.NewGuid().ToString();
            return _projectService.AdoptProject(projectInfo, fileName, id);
        }

        /// <summary>
        /// Facade for ProjectWork.DoSmartUpdateAsync
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private Task<ProjectStateDTO> UpdateParamsAsync(AdoptProjectWithParametersPayload payload)
        {
            _logger.LogInformation($"start of UpdateParamsAsync, projectName {payload.Name}");

            return _projectWork.DoSmartUpdateAsync(payload.Config, payload.Name);
        }
    }
}
