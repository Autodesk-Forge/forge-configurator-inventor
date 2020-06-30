using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;

namespace WebApplication
{
    /// <summary>
    /// Initialize data for anonymous user.
    /// </summary>
    public class Initializer
    {
        private readonly ILogger<Initializer> _logger;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly FdaClient _fdaClient;
        private readonly ProjectWork _projectWork;
        private readonly UserResolver _userResolver;
        private readonly LocalCache _localCache;
        private readonly OssBucket _bucket;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(ILogger<Initializer> logger, FdaClient fdaClient, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            ProjectWork projectWork, UserResolver userResolver, LocalCache localCache)
        {
            _logger = logger;
            _fdaClient = fdaClient;
            _projectWork = projectWork;
            _userResolver = userResolver;
            _localCache = localCache;
            _defaultProjectsConfiguration = optionsAccessor.Value;

            // bucket for anonymous user
            _bucket = _userResolver.AnonymousBucket;
        }

        public async Task InitializeAsync()
        {
            using var scope = _logger.BeginScope("Init");
            _logger.LogInformation("Initializing base data");

            // OSS bucket might fail to create, so repeat attempts
            var createBucketPolicy = Policy
                .Handle<ApiException>()
                .WaitAndRetryAsync(
                    retryCount: 4,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan) => _logger.LogWarning("Cannot create OSS bucket. Repeating")
                );

            await Task.WhenAll(
                    // create bundles and activities
                    _fdaClient.InitializeAsync(),

                    // create the bucket
                    createBucketPolicy.ExecuteAsync(async () => await _bucket.CreateAsync())
                );

            _logger.LogInformation($"Bucket {_bucket.AnonymousBucketKey} created");

            // OSS bucket might be not ready yet, so repeat attempts
            var waitForBucketPolicy = Policy
                .Handle<ApiException>(e => e.ErrorCode == StatusCodes.Status404NotFound)
                .WaitAndRetryAsync(
                    retryCount: 4,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan) => _logger.LogWarning("Cannot get fresh OSS bucket. Repeating")
                ); 

            // publish default project files (specified by the appsettings.json)
            foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
            {
                var projectUrl = defaultProjectConfig.Url;
                var project = await _userResolver.GetProjectAsync(defaultProjectConfig.Name);

                _logger.LogInformation($"Launching 'TransferData' for {projectUrl}");
                string signedUrl = await waitForBucketPolicy.ExecuteAsync(async () => await _bucket.CreateSignedUrlAsync(project.OSSSourceModel, ObjectAccess.ReadWrite));

                // TransferData from s3 to oss
                await _projectWork.FileTransferAsync(projectUrl, signedUrl);
                _logger.LogInformation($"'TransferData' for {projectUrl} is done.");

                await _projectWork.AdoptAsync(defaultProjectConfig, signedUrl);
            }

            _logger.LogInformation("Added default projects.");
        }

        public async Task ClearAsync(bool deleteUserBuckets)
        {
            try
            {
                _logger.LogInformation($"Deleting anonymous user bucket {_bucket.AnonymousBucketKey}");
                await _bucket.DeleteAsync();
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                _logger.LogInformation($"Nothing to delete because bucket {_bucket.AnonymousBucketKey} does not exists yet");
            }

            if (deleteUserBuckets) {
                _logger.LogInformation($"Deleting user buckets for registered users");
                // delete all user buckets
                var buckets = await _bucket.GetBucketsAsync();
                await _bucket.DeleteBucketsAsync(buckets);
            }

            // delete bundles and activities
            await _fdaClient.CleanUpAsync();

            // cleanup locally cached files
            Directory.Delete(_localCache.LocalRootName, true);
        }
    }
}
