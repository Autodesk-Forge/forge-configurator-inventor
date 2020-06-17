using System;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        /// <summary>
        /// Bucket key for anonymous user.
        /// </summary>
        public string BucketKey { get; }

        public Task<string> Nickname => _nickname.Value;
        private readonly Lazy<Task<string>> _nickname;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, string bucketKey = null)
        {
            var configuration = forgeConfigOptionsAccessor.Value.Validate();
            BucketKey = bucketKey ?? $"projects-{configuration.ClientId.Substring(0, 3)}-{configuration.HashString()}{Environment.GetEnvironmentVariable("BucketKeySuffix")}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }
    }
}
