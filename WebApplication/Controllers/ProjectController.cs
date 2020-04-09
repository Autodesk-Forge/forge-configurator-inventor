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
        public async Task<IEnumerable<Project>> List()
        {
            // TODO move to projects repository?

            List<ObjectDetails> objects = await _forge.GetBucketObjects(_bucketNameProvider.BucketName, $"{ONK.projectsFolder}-");
            var projects = new List<Project>();
            foreach(ObjectDetails objDetails in objects)
            {
                var nameProvider = new ObjectNameProvider(objDetails.ObjectKey, true);
                projects.Add(new Project { 
                    Id = nameProvider.ProjectName,
                    Label = nameProvider.ProjectName,
                    Image = "./bike.png" }); // temporary icon to verify control
            }
            return projects;
        }
    }
}
