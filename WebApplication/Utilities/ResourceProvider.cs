using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        private const string LocalCacheDir = "LocalCache";
        public const string VirtualCacheDir = "/data";

        private readonly IForgeOSS _forgeOSS;

        public string BucketKey { get; }

        public Task<string> Nickname => _nickname.Value;
        private readonly Lazy<Task<string>> _nickname;

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName => _localRootDir.Value;
        private readonly Lazy<string> _localRootDir;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client,
                                IForgeOSS forgeOSS, string bucketKey = null)
        {
            _forgeOSS = forgeOSS;
            var configuration = forgeConfigOptionsAccessor.Value.Validate();
            BucketKey = bucketKey ?? $"projects-{configuration.ClientId.Substring(0, 4)}-{configuration.HashString()}{Environment.GetEnvironmentVariable("BucketKeySuffix")}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));

            _localRootDir = new Lazy<string>(() =>
            {
                var localDir = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir); // TODO: should the root dir be taken from config? or another class?

                // ensure the directory is exists
                Directory.CreateDirectory(localDir);

                return localDir;
            });
        }

        /// <summary>
        /// Generate a signed URL to OSS object at <see cref="BucketKey"/>.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public Task<string> CreateSignedUrlAsync(string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            return _forgeOSS.CreateSignedUrlAsync(BucketKey, objectName, access, minutesExpiration);
        }

        /// <summary>
        /// Get URL pointing for the data file.
        /// </summary>
        /// <param name="localFileName">Full filename. Must be under "local cache root"</param>
        public string ToDataUrl(string localFileName)
        {
            if (!localFileName.StartsWith(LocalRootName, StringComparison.InvariantCultureIgnoreCase))
                throw new ApplicationException("Attempt to generate URL for non-data file");

            string relativeName = localFileName.Substring(LocalRootName.Length);
            return VirtualCacheDir + relativeName.Replace('\\', '/');
        }
    }
}
