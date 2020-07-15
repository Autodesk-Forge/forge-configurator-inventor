using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Middleware;
using WebApplication.Services;
using WebApplication.Utilities;

namespace WebApplication.State
{
    /// <summary>
    /// Business logic to differentiate state for logged in and anonymous users.
    /// </summary>
    public class UserResolver
    {
        private readonly IForgeOSS _forgeOSS;
        private readonly LocalCache _localCache;
        private readonly ILogger<UserResolver> _logger;
        private readonly IConfiguration _configuration;
        private readonly ForgeConfiguration _forgeConfig;

        private readonly Lazy<Task<dynamic>> _lazyProfile;

        /// <summary>
        /// OSS bucket for anonymous user.
        /// </summary>
        public OssBucket AnonymousBucket { get; }

        public string Token { private get; set; }
        public bool IsAuthenticated => ! string.IsNullOrEmpty(Token);

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS,
                            IOptions<ForgeConfiguration> forgeConfiguration, LocalCache localCache, ILogger<UserResolver> logger, IConfiguration configuration)
        {
            _forgeOSS = forgeOSS;
            _localCache = localCache;
            _logger = logger;
            _configuration = configuration;
            _forgeConfig = forgeConfiguration.Value;

            AnonymousBucket = new OssBucket(_forgeOSS, resourceProvider.BucketKey, logger);

            _lazyProfile = new Lazy<Task<dynamic>>(async () => await _forgeOSS.GetProfileAsync(Token));
        }

        public string GetBucketPrefix()
        {
            string suffix = _configuration?.GetValue<string>("BucketKeySuffix");
            return $"authd{suffix}-{_forgeConfig.ClientId}".ToLowerInvariant();
        }

        public async Task<OssBucket> GetBucketAsync(bool tryToCreate = false)
        {
            if (! IsAuthenticated) return AnonymousBucket;

            dynamic profile = await GetProfileAsync();
            var userId = profile.userId;

            // an OSS bucket must have a unique name, so it should be generated in a way,
            // so it a Forge user gets registered into several deployments it will not cause
            // name collisions. So use client ID (as a salt) to generate bucket name.
            var userHash = Crypto.GenerateHashString(_forgeConfig.ClientId + userId);
            var bucketKey = $"{GetBucketPrefix()}-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();

            var bucket = new OssBucket(_forgeOSS, bucketKey, _logger);
            if (tryToCreate)
            {
                // TODO: VERY INEFFECTIVE!!!!!
                try
                {
                    await bucket.CreateAsync();
                }
                catch (ApiException e) when(e.ErrorCode == StatusCodes.Status409Conflict)
                {
                    // means - the bucket already exists
                }
            }

            return bucket;
        }

        public async Task<Project> GetProjectAsync(string projectName, bool ensureDir = true)
        {
            var userFullDirName = await GetFullUserDir();
            if (ensureDir)
            {
                Directory.CreateDirectory(userFullDirName); // TODO: should not do it each time
            }
            return new Project(projectName, userFullDirName);
        }

        /// <summary>
        /// Get project storage by project name.
        /// </summary>
        public async Task<ProjectStorage> GetProjectStorageAsync(string projectName, bool ensureDir = true)
        {
            var project = await GetProjectAsync(projectName, ensureDir);
            return new ProjectStorage(project);
        }

        public Task<dynamic> GetProfileAsync() => _lazyProfile.Value;

        public async Task<string> GetFullUserDir()
        {
            string userDir;
            if (IsAuthenticated)
            {
                var profile = await GetProfileAsync();

                // generate dirname to hide Oxygen user ID
                userDir = Crypto.GenerateHashString("SDRA" + profile.userId);
            }
            else
            {
                userDir = "_anonymous";
            }

            return Path.Combine(_localCache.LocalRootName, userDir);
        }
    }
}
