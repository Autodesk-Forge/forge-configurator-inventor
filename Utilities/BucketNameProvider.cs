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

        public BucketNameProvider(IForge forge)
        {
            _forge = forge;
        }
    }
}