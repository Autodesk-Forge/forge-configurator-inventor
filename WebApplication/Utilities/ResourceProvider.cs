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
                if (string.IsNullOrEmpty(_bucketKey))
                {
                    // bucket name generated as "project-<three first chars from client ID>-<hash of client ID>"
                    _bucketKey = $"projects-{_configuration.ClientId.Substring(0, 3)}-{_configuration.HashString()}".ToLowerInvariant();
                }

                return _bucketKey;
            }
        }
        private string _bucketKey;

        private readonly Lazy<Task<string>> _nickname;
        public Task<string> Nickname => _nickname.Value;

        private readonly ForgeConfiguration _configuration;

        public ResourceProvider(IOptions<ProjectsBucket> projectsBucketOptionsAccessor, IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client)
        {
            _bucketKey = projectsBucketOptionsAccessor.Value.Name;
            _configuration = forgeConfigOptionsAccessor.Value.Validate();
            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }
    }
}
