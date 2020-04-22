﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public ProjectController(ILogger<ProjectController> logger, IForgeOSS forge, ResourceProvider resourceProvider, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _forge = forge;
            _resourceProvider = resourceProvider;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> ListAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            // TODO move to projects repository?
            List<ObjectDetails> objects = await _forge.GetBucketObjectsAsync(_resourceProvider.BucketKey, $"{ONC.ProjectsFolder}-");
            var projectDTOs = new List<ProjectDTO>();
            foreach(ObjectDetails objDetails in objects)
            {
                var project = Project.FromObjectKey(objDetails.ObjectKey);
                projectDTOs.Add(new ProjectDTO { 
                    Id = project.Name,
                    Label = project.Name,
                    Image = project.HrefThumbnail });

                var thumbnailUrl = await _forge.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.Attributes.Thumbnail);
                var localFile = Path.Combine(Directory.GetCurrentDirectory(), "LocalCache", project.Attributes.Thumbnail);
                await httpClient.Download(thumbnailUrl, localFile);
            }
            return projectDTOs;
        }
    }
}
