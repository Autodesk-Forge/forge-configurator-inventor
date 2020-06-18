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
using WebApplication.Services;
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
                Project project = projectStorage.Project;

                var dto = _dtoGenerator.MakeProjectDTO<ProjectDTO>(project, projectStorage.Metadata.Hash);
                dto.Id = project.Name;
                dto.Label = project.Name;
                dto.Image = _localCache.ToDataUrl(project.LocalAttributes.Thumbnail);

                projectDTOs.Add(dto);
            }

            return projectDTOs.OrderBy(project => project.Label);
        }

        [HttpPost]
        public async Task CreateProject([FromForm]NewProjectModel projectModel)
        {
            var bucket = await _userResolver.GetBucket(true);
            var project = await _userResolver.GetProjectAsync(projectModel.package.FileName.ToLowerInvariant()); //TODO: we need to sanitize filename and check if project already exists

            var projectInfo = new ProjectInfo
            {
                Name = projectModel.package.FileName.ToLowerInvariant(),
                TopLevelAssembly = projectModel.root
            };
            
            using var file = new TempFile();
            using var fileWriteStream = System.IO.File.OpenWrite(file.Name);
            await projectModel.package.CopyToAsync(fileWriteStream);
            fileWriteStream.Close();
            
            using var fileReadStream = System.IO.File.OpenRead(file.Name);

            await bucket.UploadObjectAsync(project.OSSSourceModel, fileReadStream);
            
            string signedUrl = await bucket.CreateSignedUrlAsync(project.OSSSourceModel, ObjectAccess.Read);

            await _projectWork.AdoptAsync(projectInfo, signedUrl);
        }
    }
}
