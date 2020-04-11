using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        public string BucketName
        {
            get
            {
                if (_bucketName == null)
                {
                    // bucket name generated as "project-<three first chars from client ID>-<hash of client ID>"
                    _bucketName = $"projects-{_configuration.ClientId.Substring(0, 3)}-{_configuration.HashString()}".ToLowerInvariant();
                }

                return _bucketName;
            }
        }
        private string _bucketName;

        private string _nickname;

        private readonly ForgeConfiguration _configuration;
        private readonly DesignAutomationClient _client;

        public ResourceProvider(IOptions<ForgeConfiguration> optionsAccessor, DesignAutomationClient client)
        {
            _configuration = optionsAccessor.Value.Validate();
            _client = client;
        }

        public async Task<string> GetNicknameAsync()
        {
            if (_nickname == null)
            {
                _nickname = await _client.GetNicknameAsync("me");
            }

            return _nickname;
        }
    }
}
