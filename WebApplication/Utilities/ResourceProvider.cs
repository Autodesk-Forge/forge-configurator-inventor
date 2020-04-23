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

        public string BucketKey { get; }

        public Task<string> Nickname => _nickname.Value;
        private readonly Lazy<Task<string>> _nickname;

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName => _localRootDir.Value;
        private readonly Lazy<string> _localRootDir;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, string bucketKey = null)
        {
            var configuration = forgeConfigOptionsAccessor.Value.Validate();
            BucketKey = bucketKey ?? $"projects-{configuration.ClientId.Substring(0, 4)}-{configuration.HashString()}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));

            _localRootDir = new Lazy<string>(() =>
            {
                var localDir = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir); // TODO: should the root dir be taken from config? or another class?

                // ensure the directory is exists
                Directory.CreateDirectory(localDir);

                return localDir;
            });
        }
    }
}
