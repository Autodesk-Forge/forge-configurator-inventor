using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Services.Exceptions;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public class ProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly UserResolver _userResolver;
        private readonly Uploads _uploads;

        public ProjectService(ILogger<ProjectService> logger, UserResolver userResolver, Uploads uploads)
        {
            _logger = logger;
            _userResolver = userResolver;
            _uploads = uploads;
        }

        public async Task<string> CreateProject(NewProjectModel projectModel)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectModel.package.FileName);
            var bucket = await _userResolver.GetBucketAsync(true);

            // Check if project already exists
            var projectNames = await GetProjectNamesAsync(bucket);
            foreach (var existingProjectName in projectNames)
            {
                if (projectName == existingProjectName) 
                    throw new ProjectAlreadyExistsException(projectName);
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

            return packageId;
        }

        /// <summary>
        /// Get list of project names for a bucket.
        /// </summary>
        //TODO: make private again
        public async Task<ICollection<string>> GetProjectNamesAsync(OssBucket bucket)
        {
            var objectDetails = (await bucket.GetObjectsAsync(ONC.ProjectsMask));
            var projectNames = objectDetails
                .Select(objDetails => ONC.ToProjectName(objDetails.ObjectKey))
                .ToList();
            return projectNames;
        }
    }
}
