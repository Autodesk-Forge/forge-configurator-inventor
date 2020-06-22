using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.Processing;
using WebApplication.State;
using WebApplication.Utilities;
using Project = WebApplication.State.Project;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly DtoGenerator _dtoGenerator;
        private readonly UserResolver _userResolver;
        private readonly LocalCache _localCache;
        private readonly ProjectWork _projectWork;

        public ProjectsController(ILogger<ProjectsController> logger, DtoGenerator dtoGenerator, UserResolver userResolver,
            LocalCache localCache, ProjectWork projectWork)
        {
            _logger = logger;
            _dtoGenerator = dtoGenerator;
            _userResolver = userResolver;
            _localCache = localCache;
            _projectWork = projectWork;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> ListAsync()
        {
            var bucket = await _userResolver.GetBucket(tryToCreate: false); // TODO: remove before PR

            // TODO move to projects repository?
            List<ObjectDetails> objects = await bucket.GetObjectsAsync($"{ONC.ProjectsFolder}-");

            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var projectName = ONC.ToProjectName(objDetails.ObjectKey);

                ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName); // TODO: expensive to do it in the loop

                projectDTOs.Add(ToDTO(projectStorage));
            }

            return projectDTOs;
        }

        [HttpPost]
        public async Task<ProjectDTO> CreateProject([FromForm]NewProjectModel projectModel)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectModel.package.FileName);

            // TODO: check if project already exists

            var projectInfo = new ProjectInfo
            {
                Name = projectName,
                TopLevelAssembly = projectModel.root
            };
            
            // download file locally (a place to improve... would be good to stream it directly to OSS)
            using var file = new TempFile();
            await using (var fileWriteStream = System.IO.File.OpenWrite(file.Name))
            {
                await projectModel.package.CopyToAsync(fileWriteStream);
            }

            // update the file to OSS
            var bucket = await _userResolver.GetBucket(true);
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName);

            string ossSourceModel = projectStorage.Project.OSSSourceModel;
            await using (var fileReadStream = System.IO.File.OpenRead(file.Name))
            {
                await bucket.UploadObjectAsync(ossSourceModel, fileReadStream);
            }

            // adopt the project
            string signedUrl = await bucket.CreateSignedUrlAsync(ossSourceModel);
            await _projectWork.AdoptAsync(projectInfo, signedUrl);

            return ToDTO(projectStorage);
        }

        /// <summary>
        /// Generate project DTO.
        /// </summary>
        private ProjectDTO ToDTO(ProjectStorage projectStorage)
        {
            Project project = projectStorage.Project;

            var dto = _dtoGenerator.MakeProjectDTO<ProjectDTO>(project, projectStorage.Metadata.Hash);
            dto.Id = project.Name;
            dto.Label = project.Name;
            dto.Image = _localCache.ToDataUrl(project.LocalAttributes.Thumbnail);
            return dto;
        }
    }
}
