using System.Collections.Generic;
using System.IO;
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
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IForgeOSS _forge;
        private readonly ResourceProvider _resourceProvider;
        private readonly LocalStorage _localStorage;

        public ProjectController(ILogger<ProjectController> logger, IForgeOSS forge, ResourceProvider resourceProvider, LocalStorage localStorage)
        {
            _logger = logger;
            _forge = forge;
            _resourceProvider = resourceProvider;
            _localStorage = localStorage;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> ListAsync()
        {
            // TODO move to projects repository?
            List<ObjectDetails> objects = await _forge.GetBucketObjectsAsync(_resourceProvider.BucketKey, $"{ONC.ProjectsFolder}-");
            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var project = Project.FromObjectKey(objDetails.ObjectKey);
                //await _localStorage.EnsureLocalAsync(httpClient, project);
                
                projectDTOs.Add(new ProjectDTO { 
                                    Id = project.Name,
                                    Label = project.Name,
                                    Image = project.HrefThumbnail
                                });
            }
            return projectDTOs;
        }
    }
}
