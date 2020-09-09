/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Services;
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
        private readonly DtoGenerator _dtoGenerator;
        private readonly UserResolver _userResolver;
        private readonly ProfileProvider _profileProvider;
        private readonly Uploads _uploads;

        public ProjectsController(ILogger<ProjectsController> logger, DtoGenerator dtoGenerator, UserResolver userResolver, ProfileProvider profileProvider, Uploads uploads)
        {
            _logger = logger;
            _dtoGenerator = dtoGenerator;
            _userResolver = userResolver;
            _profileProvider = profileProvider;
            _uploads = uploads;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProjectDTO>> ListAsync()
        {
            var bucket = await _userResolver.GetBucketAsync(tryToCreate: true);

            var projectDTOs = new List<ProjectDTO>();
            foreach (var projectName in await GetProjectNamesAsync(bucket))
            {
                // TODO: in future bad projects should not affect project listing. It's a workaround
                try
                {
                    ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName); // TODO: expensive to do it in the loop

                    // handle situation when project is not cached locally
                    await projectStorage.EnsureAttributesAsync(bucket, ensureDir: true);

                    projectDTOs.Add(_dtoGenerator.ToDTO(projectStorage));
                }
                catch (Exception e)
                {
                    // log, swallow and continue (see the comment above)
                    _logger.LogWarning(e, $"Ignoring '{projectName}' project, which (seems) failed to adopt.");
                }
            }

            return projectDTOs;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromForm]NewProjectModel projectModel)
        {
            if (!_profileProvider.IsAuthenticated)
            {
                _logger.LogError("Attempt to create project for anonymous user");
                return BadRequest();
            }

            var projectName = Path.GetFileNameWithoutExtension(projectModel.package.FileName);
            var bucket = await _userResolver.GetBucketAsync(true);

            // Check if project already exists
            foreach (var existingProjectName in await GetProjectNamesAsync(bucket))
            {
                if (projectName == existingProjectName)
                {
                    _logger.LogError($"Found existing '{projectName}' project");
                    return Conflict();
                }
            }

            var projectInfo = new ProjectInfo
            {
                Name = projectName,
                TopLevelAssembly = projectModel.root
            };
            
            // download file locally (a place to improve... would be good to stream it directly to OSS)
            var fileName = Path.GetTempFileName();
            await using (var fileWriteStream = System.IO.File.OpenWrite(fileName))
            {
                await projectModel.package.CopyToAsync(fileWriteStream);
            }

            var packageId = Guid.NewGuid().ToString();
            _uploads.AddUploadData(packageId, projectInfo, fileName);

            return Ok(packageId);

        }

        [HttpDelete]
        public async Task<StatusCodeResult> DeleteProjects([FromBody] List<string> projectNameList)
        {
            if (!_profileProvider.IsAuthenticated)
            {
                _logger.LogError("Attempt to delete projects for anonymous user");
                return BadRequest();
            }

            var bucket = await _userResolver.GetBucketAsync(true);

            // collect all oss objects for all provided projects
            var tasks = new List<Task>();

            foreach (var projectName in projectNameList)
            {
                tasks.Add(bucket.DeleteObjectAsync(Project.ExactOssName(projectName)));

                foreach (var searchMask in ONC.ProjectFileMasks(projectName))
                {
                    var objects = await bucket.GetObjectsAsync(searchMask);
                    foreach (var objectDetail in objects)
                    {
                        tasks.Add(bucket.DeleteObjectAsync(objectDetail.ObjectKey));
                    }
                }
            }

            // delete the OSS objects
            await Task.WhenAll(tasks);
            for (var i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].IsFaulted)
                {
                    _logger.LogError($"Failed to delete file #{i}");
                }
            }

            // delete local cache for all provided projects
            foreach (var projectName in projectNameList)
            {
                var projectStorage = await _userResolver.GetProjectStorageAsync(projectName, ensureDir: false);
                projectStorage.DeleteLocal();
            }

            return NoContent();
        }

        /// <summary>
        /// Get list of project names for a bucket.
        /// </summary>
        private async Task<ICollection<string>> GetProjectNamesAsync(OssBucket bucket)
        {
            return (await bucket.GetObjectsAsync(ONC.ProjectsMask))
                                .Select(objDetails =>  ONC.ToProjectName(objDetails.ObjectKey))
                                .ToList();
        }
    }
}
