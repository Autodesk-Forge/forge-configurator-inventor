using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using SalesDemoToolApp.Utilities;

namespace IoConfigDemo
{
    public class BucketNameProvider
    {
        public string BucketName
        {
            get
            {
                if (_bucketName == null)
                {
                    var configuration = _forge.Configuration;
                    // bucket name generated as "project-<three first chars from client ID>-<hash of client ID>"
                    _bucketName = $"projects-{configuration.ClientId.Substring(0, 3)}-{configuration.HashString()}".ToLowerInvariant();
                }

                return _bucketName;
            }
        }
        private string _bucketName;
        private readonly IForge _forge;
        private readonly DesignAutomationClient _client;

        private string _nickname;


        public BucketNameProvider(IForge forge, DesignAutomationClient client)
        {
            _forge = forge;
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
