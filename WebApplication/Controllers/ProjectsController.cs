using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.Processing;
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
        private readonly LocalCache _localCache;
        private readonly ProjectWork _projectWork;

        public ProjectsController(ILogger<ProjectsController> logger, DtoGenerator dtoGenerator, UserResolver userResolver,
            LocalCache localCache, ProjectWork projectWork)
        {
            _logger = logger;
            _dtoGenerator = dtoGenerator;
            _userResolver = userResolver;
            _localCache = localCache;
            _projectWork = projectWork;
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

                    projectDTOs.Add(ToDTO(projectStorage));
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
            if (!_userResolver.IsAuthenticated)
            {
                _logger.LogError("Attempt to create project for anonymous user");
                return BadRequest();
            }

            var projectName = Path.GetFileNameWithoutExtension(projectModel.package.FileName);
            var bucket = await _userResolver.GetBucketAsync(true);

            // Check if project already exists
            foreach (var existingProjectName in await GetProjectNamesAsync(bucket))
            {
                if (projectName == existingProjectName) return Conflict();
            }

            var projectInfo = new ProjectInfo
            {
                Name = projectName,
                TopLevelAssembly = projectModel.root
            };
            
            // download file locally (a place to improve... would be good to stream it directly to OSS)
            using var file = new TempFile();
            await using (var fileWriteStream = System.IO.File.OpenWrite(file.Name))
            {
                await projectModel.package.CopyToAsync(fileWriteStream);
            }

            // upload the file to OSS
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectName);

            string ossSourceModel = projectStorage.Project.OSSSourceModel;
            await bucket.SmartUploadAsync(file.Name, ossSourceModel);

            // adopt the project
            bool adopted = false;
            try
            {
                string signedUrl = await bucket.CreateSignedUrlAsync(ossSourceModel);
                await _projectWork.AdoptAsync(projectInfo, signedUrl);

                adopted = true;
            }
            catch (FdaProcessingException fpe)
            {
                var result = new ResultDTO
                {
                    Success = false,
                    Message = fpe.Message,
                    ReportUrl = fpe.ReportUrl
                };
                return UnprocessableEntity(result);
            }
            finally
            {
                // on any failure during adoption we consider that project adoption failed and it's not usable
                if (! adopted)
                {
                    _logger.LogInformation($"Adoption failed. Removing '{ossSourceModel}' OSS object.");
                    await bucket.DeleteObjectAsync(ossSourceModel);
                }
            }

            return Ok(ToDTO(projectStorage));
        }

        [HttpDelete]
        public async Task<StatusCodeResult> DeleteProjects([FromBody] List<string> projectNameList)
        {
            if (!_userResolver.IsAuthenticated)
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

                foreach (var searchMask in ONC.ProjectMasks(projectName))
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
        /// Generate project DTO.
        /// </summary>
        private ProjectDTO ToDTO(ProjectStorage projectStorage)
        {
            Project project = projectStorage.Project;

            var dto = _dtoGenerator.MakeProjectDTO<ProjectDTO>(project, projectStorage.Metadata.Hash);
            dto.Id = project.Name;
            dto.Label = project.Name;
            dto.Image = _localCache.ToDataUrl(project.LocalAttributes.Thumbnail);
            return dto;
        }

        /// <summary>
        /// Get list of project names for a bucket.
        /// </summary>
        private async Task<ICollection<string>> GetProjectNamesAsync(OssBucket bucket)
        {
            return (await bucket.GetObjectsAsync($"{ONC.ProjectsFolder}-"))
                                .Select(objDetails =>  ONC.ToProjectName(objDetails.ObjectKey))
                                .ToList();
        }
    }
}
