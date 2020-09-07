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
        private readonly DtoGenerator _dtoGenerator;

        public AdoptProjectService(ILogger<AdoptProjectService> logger, ProjectService projectService, 
            Uploads uploads, ProjectWork projectWork, UserResolver userResolver, DtoGenerator dtoGenerator)
        {
            _logger = logger;
            _projectService = projectService;
            _uploads = uploads;
            _projectWork = projectWork;
            _userResolver = userResolver;
            _dtoGenerator = dtoGenerator;
        }

        /// <summary>
        /// https://jira.autodesk.com/browse/INVGEN-45256
        /// </summary>
        /// <param name="payload">project configuration with parameters</param>
        public async Task<string> AdoptProjectWithParametersAsync(AdoptProjectWithParametersPayload payload)
        {
            //TODO: check whether project already exists. If yes, skip upload and adoption and continue with parameters update.
            var packageId = CreateProjectAsync(payload).Result;

            (ProjectInfo projectInfo, string fileName) = _uploads.GetUploadData(packageId);
            _uploads.ClearUploadData(packageId);

            var id = Guid.NewGuid().ToString();
            await _projectService.AdoptProject(projectInfo, fileName, id);

            await _projectWork.DoSmartUpdateAsync(payload.Config, payload.Name);

            return packageId;
        }

        /// <summary>
        /// Kind of a facade for ProjectService.CreateProject
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private async Task<string> CreateProjectAsync(DefaultProjectConfiguration configuration)
        {
            _logger.LogInformation($"adopting project {configuration.Name}");

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
    }
}
