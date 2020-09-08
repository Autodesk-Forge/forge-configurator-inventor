using System.Threading.Tasks;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public class MigrationBucketKeyProvider : IBucketKeyProvider
    {
        private readonly BucketPrefixProvider _bucketPrefixProvider;
        private readonly ResourceProvider _resourceProvider;
        private string BucketKey = "";
        public string AnonymousBucketKey {get;}

        public MigrationBucketKeyProvider(BucketPrefixProvider bucketPrefixProvider, ResourceProvider resourceProvider)
        {
            _bucketPrefixProvider = bucketPrefixProvider;
            _resourceProvider = resourceProvider;
            AnonymousBucketKey = resourceProvider.BucketKey;
        }

        public Task<string> GetBucketKeyAsync()
        {
            return Task.FromResult(BucketKey);
        }
        public string SetBucketKeyFromOld(string bucketKeyOld)
        {
            string [] splittedBucketKeyOld = bucketKeyOld.Split('-');

            if (splittedBucketKeyOld[0] == "projects")
            {
                // anonymous bucket key
                BucketKey = AnonymousBucketKey;
            }
            else
            {
                // logged user bucket key
                string userId = splittedBucketKeyOld[2];
                string userHash = splittedBucketKeyOld[3];
                BucketKey = _resourceProvider.LoggedUserBucketKey(userId, userHash);
            }

            return BucketKey;
        }
    }
}
