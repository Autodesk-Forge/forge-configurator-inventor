using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoConfigDemo.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IForge _forge;

        private readonly BucketNameProvider _bucketNameProvider;

        public ProjectController(ILogger<ProjectController> logger, IForge forge, BucketNameProvider bucketNameProvider)
        {
            _logger = logger;
            _forge = forge;
            _bucketNameProvider = bucketNameProvider;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> List()
        {
            // TODO move to projects repository?

            List<ObjectDetails> objects = await _forge.GetBucketObjects(_bucketNameProvider.BucketName, $"{ONC.projectsFolder}-");
            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var project = Project.FromBucketKey(objDetails.ObjectKey);
                projectDTOs.Add(new ProjectDTO { 
                    Id = project.Name,
                    Label = project.Name,
                    Image = project.Thumbnail });
            }
            return projectDTOs;
        }
    }
}
