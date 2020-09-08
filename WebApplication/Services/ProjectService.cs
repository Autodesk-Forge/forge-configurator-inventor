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
        
        //TODO: remove duplicity, use in AdoptJob
        public async Task<ProjectStorage> AdoptProject(ProjectInfo projectInfo, string fileName, string id)
        {
            using var scope = _logger.BeginScope("Project Adoption ({id})");

            _logger.LogInformation($"Adopt {id} for project {projectInfo.Name} started.");

            // upload the file to OSS
            var bucket = await _userResolver.GetBucketAsync(true);
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectInfo.Name);

            string ossSourceModel = projectStorage.Project.OSSSourceModel;

            await bucket.SmartUploadAsync(fileName, ossSourceModel);

            // cleanup before adoption
            File.Delete(fileName);

            // adopt the project
            bool adopted = false;
            try
            {
                string signedUploadedUrl = await bucket.CreateSignedUrlAsync(ossSourceModel);

                await _projectWork.AdoptAsync(projectInfo, signedUploadedUrl);

                adopted = true;
            }
            catch (FdaProcessingException fpe)
            {
                throw;
                //await resultSender.SendErrorAsync(id, fpe.ReportUrl);
            }
            finally
            {
                // on any failure during adoption we consider that project adoption failed and it's not usable
                if (!adopted)
                {
                    _logger.LogInformation($"Adoption failed. Removing '{ossSourceModel}' OSS object.");
                    await bucket.DeleteObjectAsync(ossSourceModel);
                }
            }

            _logger.LogInformation($"Adopt {id} for project {projectInfo.Name} completed.");
            //await resultSender.SendSuccessAsync(_dtoGenerator.ToDTO(projectStorage));

            return projectStorage;
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
    }
}
