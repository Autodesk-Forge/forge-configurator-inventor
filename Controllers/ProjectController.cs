using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SalesDemoToolApp.Utilities;

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

            List<ObjectDetails> objects = await _forge.GetBucketObjects(_bucketNameProvider.BucketName);
            var projects = new List<Project>();
            foreach(ObjectDetails objDetails in objects)
            {
                projects.Add(new Project { 
                    Id = objDetails.ObjectKey,
                    Label = objDetails.ObjectKey,
                    Image = "./bike.png" }); // temporary icon to verify control
            }
            return projects;
        }
    }
}
