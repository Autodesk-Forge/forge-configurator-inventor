using System;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        public string BucketKey
        {
            get
            {
                return _bucketKey;
            }
        }
        private string _bucketKey;

        private readonly Lazy<Task<string>> _nickname;
        public Task<string> Nickname => _nickname.Value;

        private readonly ForgeConfiguration _configuration;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, string bucketName = null)
        {
            _configuration = forgeConfigOptionsAccessor.Value.Validate();
            _bucketKey = bucketName ?? $"projects-{_configuration.ClientId.Substring(0, 3)}-{_configuration.HashString()}".ToLowerInvariant();
            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }
    }
}
