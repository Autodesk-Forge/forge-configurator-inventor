using System;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        public string BucketKey { get; }

        private readonly Lazy<Task<string>> _nickname;
        public Task<string> Nickname => _nickname.Value;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, string bucketKey = null)
        {
            var configuration = forgeConfigOptionsAccessor.Value.Validate();
            BucketKey = bucketKey ?? $"projects-{configuration.ClientId.Substring(0, 4)}-{configuration.HashString()}".ToLowerInvariant();
            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }
    }
}
