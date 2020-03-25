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

        /// <summary>
        /// Generated bucket name.
        /// </summary>
        private string BucketName
        {
            get
            {
                if (_bucketName == null)
                {
                    var configuration = _forge.Configuration;
                    // bucket name generated as "project-<three first chars from client ID>-<hash of client ID>"
                    _bucketName = $"projects-{configuration.ClientId.Substring(0, 3)}-{configuration.HashString()}".ToLowerInvariant();

                    _logger.LogInformation($"Using '{_bucketName}' bucket.");
                }

                return _bucketName;
            }
        }
        private string _bucketName;

        public ProjectController(ILogger<ProjectController> logger, IForge forge)
        {
            _logger = logger;
            _forge = forge;
        }

        [HttpGet("")]
        public async Task<IEnumerable<Project>> List()
        {
            // TODO move to projects repository?

            List<ObjectDetails> objects = await _forge.GetBucketObjects(BucketName);
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
