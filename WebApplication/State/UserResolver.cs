using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
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
        private readonly ForgeConfiguration _forgeConfig;

        /// <summary>
        /// OSS bucket for anonymous user.
        /// </summary>
        public OssBucket AnonymousBucket { get; }

        public string Token { private get; set; }
        public bool IsAuthenticated => ! string.IsNullOrEmpty(Token);

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS, IOptions<ForgeConfiguration> forgeConfiguration, LocalCache localCache)
        {
            _forgeOSS = forgeOSS;
            _localCache = localCache;
            _forgeConfig = forgeConfiguration.Value;

            AnonymousBucket = new OssBucket(_forgeOSS, resourceProvider.BucketKey);
        }

        public async Task<OssBucket> GetBucket()
        {
            if (! IsAuthenticated) return AnonymousBucket;

            var profile = await _forgeOSS.GetProfileAsync(Token); // TODO: cache it
            var userId = profile.userId;

            // an OSS bucket must have a unique name, so it should be generated in a way,
            // so it a Forge user gets registered into several deployments it will not cause
            // name collisions. So use client ID (as a salt) to generate bucket name.
            var userHash = Crypto.GenerateHashString(_forgeConfig.ClientId + userId);
            var bucketKey = $"authd-{_forgeConfig.ClientId}-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();

            var bucket = new OssBucket(_forgeOSS, bucketKey);
            await bucket.CreateAsync();

            return bucket;
        }

        public async Task<Project> GetProject(string projectName)
        {
            string userDir;
            if (IsAuthenticated)
            {
                var profile = await _forgeOSS.GetProfileAsync(Token); // TODO: cache it

                // generate dirname to hide Oxygen user ID
                userDir = Crypto.GenerateHashString("SDRA" + profile.userId);
            }
            else
            {
                userDir = "_anonymous";
            }

            var userFullDirName = Path.Combine(_localCache.LocalRootName, userDir);
            Directory.CreateDirectory(userFullDirName); // TODO: should not do it each time
            return new Project(projectName, userFullDirName);
        }

        /// <summary>
        /// Get project storage by project name.
        /// </summary>
        public async Task<ProjectStorage> GetProjectStorage(string projectName)
        {
            var project = await GetProject(projectName);
            return new ProjectStorage(project);
        }
    }
}