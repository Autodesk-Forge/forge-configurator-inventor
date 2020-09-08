using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Services.Exceptions;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public class ProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly UserResolver _userResolver;
        private readonly ProjectWork _projectWork;
        private readonly RetryPolicy _waitForBucketPolicy;

        public ProjectService(ILogger<ProjectService> logger, UserResolver userResolver, ProjectWork projectWork)
        {
            _logger = logger;
            _userResolver = userResolver;
            _projectWork = projectWork;

            _waitForBucketPolicy = Policy
                .Handle<ApiException>(e => e.ErrorCode == StatusCodes.Status404NotFound)
                .WaitAndRetryAsync(
                    retryCount: 4,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan) => _logger.LogWarning("Cannot get fresh OSS bucket. Repeating")
                );
        }

        /// <summary>
        /// https://jira.autodesk.com/browse/INVGEN-45256
        /// </summary>
        /// <param name="payload">project configuration with parameters</param>
        /// <returns>project storage</returns>
        public async Task<ProjectStorage> AdoptProjectWithParametersAsync(AdoptProjectWithParametersPayload payload)
        {
            if (!await DoesProjectAlreadyExistAsync(payload.Name))
            {
                var bucket = await _userResolver.GetBucketAsync();
                var signedUrl = await TransferProjectToOssAsync(bucket, payload);
                await _projectWork.AdoptAsync(payload, signedUrl);
            }
            else
            {
                _logger.LogInformation($"project with name {payload.Name} already exists");
            }

            await _projectWork.DoSmartUpdateAsync(payload.Config, payload.Name);

            return await _userResolver.GetProjectStorageAsync(payload.Name);
        }

        private async Task<bool> DoesProjectAlreadyExistAsync(string projectName)
        {
            var existingProjects = await GetProjectNamesAsync();

            return existingProjects.Contains(projectName);
        }
        public async Task<string> TransferProjectToOssAsync(OssBucket bucket, DefaultProjectConfiguration projectConfig)
        {
            _logger.LogInformation($"Bucket {bucket.BucketKey} created");

            var projectUrl = projectConfig.Url;
            var project = await _userResolver.GetProjectAsync(projectConfig.Name);

            _logger.LogInformation($"Launching 'TransferData' for {projectUrl}");

            // OSS bucket might be not ready yet, so repeat attempts
            string signedUrl = await _waitForBucketPolicy.ExecuteAsync(async () =>
                await bucket.CreateSignedUrlAsync(project.OSSSourceModel, ObjectAccess.ReadWrite));

            // TransferData from s3 to temporary oss url
            await _projectWork.FileTransferAsync(projectUrl, signedUrl);

            _logger.LogInformation($"'TransferData' for {projectUrl} is done.");

            return signedUrl;
        }
        
        /// <summary>
        /// Get list of project names for a bucket.
        /// </summary>
        public async Task<ICollection<string>> GetProjectNamesAsync(OssBucket bucket = null)
        {
            bucket ??= await _userResolver.GetBucketAsync(true);

            var objectDetails = (await bucket.GetObjectsAsync(ONC.ProjectsMask));
            var projectNames = objectDetails
                .Select(objDetails => ONC.ToProjectName(objDetails.ObjectKey))
                .ToList();
            return projectNames;
        }

        public async Task DeleteProjects(ICollection<string> projectNameList, OssBucket bucket = null)
        {
            bucket ??= await _userResolver.GetBucketAsync(true);

            _logger.LogInformation($"deleting projects [{string.Join(", ", projectNameList)}] from bucket {bucket.BucketKey}");

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
        }

        public async Task DeleteAllProjects()
        {
            var projectsNames = await GetProjectNamesAsync();
            await DeleteProjects(projectsNames);
        }
    }
}
