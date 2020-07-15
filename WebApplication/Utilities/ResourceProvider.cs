using System;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Configuration;
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

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, IConfiguration configuration, string bucketKey = null)
        {
            var forgeConfiguration = forgeConfigOptionsAccessor.Value.Validate();
            string suffix = configuration != null ? configuration.GetValue<string>("BucketKeySuffix") : "";
            BucketKey = bucketKey ?? $"projects-{forgeConfiguration.ClientId.Substring(0, 3)}-{forgeConfiguration.HashString()}{suffix}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }
    }
}
