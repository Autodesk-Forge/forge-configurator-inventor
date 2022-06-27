using System;
using System.Threading.Tasks;
using webapplication.Utilities;

namespace webapplication.Services
{
    public class MigrationBucketKeyProvider : IBucketKeyProvider
    {
        private readonly BucketPrefixProvider _bucketPrefixProvider;
        private readonly IResourceProvider _resourceProvider;
        private string? BucketKey = "";
        public string? AnonymousBucketKey {get;}

        public MigrationBucketKeyProvider(BucketPrefixProvider bucketPrefixProvider, IResourceProvider resourceProvider)
        {
            _bucketPrefixProvider = bucketPrefixProvider;
            _resourceProvider = resourceProvider;
            AnonymousBucketKey = resourceProvider.BucketKey;
        }

        public Task<string?> GetBucketKeyAsync()
        {
            return Task.FromResult(BucketKey);
        }
        public string? SetBucketKeyFromOld(string? bucketKeyOld)
        {
            BucketKey = GetBucketKeyFromOld(bucketKeyOld);

            return BucketKey;
        }

        public string? GetBucketKeyFromOld(string? bucketKeyOld)
        {
            string? bucketKeyNew;
            string[]? splittedBucketKeyOld = bucketKeyOld?.Split('-');

            if (splittedBucketKeyOld?[0] == ResourceProvider.projectsTag)
            {
                // anonymous bucket key
                bucketKeyNew = AnonymousBucketKey;
            }
            else
            {
                // logged user bucket key
                string? userId = splittedBucketKeyOld?[2];
                string? userHash = splittedBucketKeyOld?[3];
                bucketKeyNew = _resourceProvider.LoggedUserBucketKey(userId, userHash);
            }

            return bucketKeyNew;
        }
    }
}
