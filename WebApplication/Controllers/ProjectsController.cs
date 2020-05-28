using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly IForgeOSS _forge;
        private readonly ResourceProvider _resourceProvider;
        private readonly DtoGenerator _dtoGenerator;

        public ProjectsController(ILogger<ProjectsController> logger, IForgeOSS forge, ResourceProvider resourceProvider, DtoGenerator dtoGenerator)
        {
            _logger = logger;
            _forge = forge;
            _resourceProvider = resourceProvider;
            _dtoGenerator = dtoGenerator;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> ListAsync()
        {
            // TODO move to projects repository?
            List<ObjectDetails> objects = await _forge.GetBucketObjectsAsync(_resourceProvider.BucketKey, $"{ONC.ProjectsFolder}-");
            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var projectName = ONC.ToProjectName(objDetails.ObjectKey);

                ProjectStorage projectStorage = _resourceProvider.GetProjectStorage(projectName);
                Project project = projectStorage.Project;

                var dto = _dtoGenerator.MakeProjectDTO<ProjectDTO>(project, projectStorage.Metadata.Hash);
                dto.Id = project.Name;
                dto.Label = project.Name;
                dto.Image = _resourceProvider.ToDataUrl(project.LocalAttributes.Thumbnail);

                projectDTOs.Add(dto);
            }

            return projectDTOs.OrderBy(project => project.Label);
        }
    }
}
