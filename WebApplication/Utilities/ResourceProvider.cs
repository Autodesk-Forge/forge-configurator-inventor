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

        /// <summary>
        /// Bucket key for anonymous user.
        /// </summary>
        public string BucketKey { get; }

        public Task<string> Nickname => _nickname.Value;
        private readonly Lazy<Task<string>> _nickname;

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir);

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, string bucketKey = null)
        {
            var configuration = forgeConfigOptionsAccessor.Value.Validate();
            BucketKey = bucketKey ?? $"projects-{configuration.ClientId.Substring(0, 3)}-{configuration.HashString()}{Environment.GetEnvironmentVariable("BucketKeySuffix")}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
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
