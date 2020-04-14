using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IForge _forge;

        private readonly ResourceProvider _resourceProvider;
        private readonly FdaClient _fda;

        public ProjectController(ILogger<ProjectController> logger, IForge forge, ResourceProvider resourceProvider, FdaClient fda)
        {
            _logger = logger;
            _forge = forge;
            _resourceProvider = resourceProvider;
            _fda = fda;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> List()
        {
            // ER: TODO: remove after completion
#if false
            string inventorDocUrl = "http://testipt.s3-us-west-2.amazonaws.com/PictureInFormTest.ipt";
            string outputUrl = await _forge.CreateDestinationUrl(_resourceProvider.BucketName, $"{Guid.NewGuid():N}.json");
            await _fda.GenerateSVF(inventorDocUrl, outputUrl);
#endif

            // TODO move to projects repository?

            List<ObjectDetails> objects = await _forge.GetBucketObjects(_resourceProvider.BucketName, $"{ONC.projectsFolder}-");
            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var project = Project.FromObjectKey(objDetails.ObjectKey);
                projectDTOs.Add(new ProjectDTO { 
                    Id = project.Name,
                    Label = project.Name,
                    Image = project.HrefThumbnail });
            }
            return projectDTOs;
        }
    }
}
