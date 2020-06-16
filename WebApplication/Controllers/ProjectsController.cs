using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
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
        private readonly ResourceProvider _resourceProvider;
        private readonly DtoGenerator _dtoGenerator;
        private readonly UserResolver _userResolver;

        public ProjectsController(ILogger<ProjectsController> logger, ResourceProvider resourceProvider, DtoGenerator dtoGenerator, UserResolver userResolver)
        {
            _logger = logger;
            _resourceProvider = resourceProvider;
            _dtoGenerator = dtoGenerator;
            _userResolver = userResolver;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> ListAsync()
        {
            var bucket = await _userResolver.GetBucket();

            // TODO move to projects repository?
            List<ObjectDetails> objects = await bucket.GetObjectsAsync($"{ONC.ProjectsFolder}-");

            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var projectName = ONC.ToProjectName(objDetails.ObjectKey);

                ProjectStorage projectStorage = await _userResolver.GetProjectStorage(projectName); // TODO: expensive to do it in the loop
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
