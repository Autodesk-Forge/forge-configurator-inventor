using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoConfigDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private Forge _forge;

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
            _forge = new Forge(); // TODO inject for unit testing?
        }

        [HttpGet]
        public async Task<IEnumerable<Project>> Get()
        {
            // TODO move to projects repository?
            string projectsBucketKey = "io-config-demo-projects"; // TODO get from config
            List<ObjectDetails> objects = await _forge.GetBucketObjects(projectsBucketKey);
            List<Project> projects = new List<Project>();
            foreach(ObjectDetails objDetails in objects)
            {
                projects.Add(new Project { Name = objDetails.ObjectKey });
            }
            return projects;
        }
    }
}
