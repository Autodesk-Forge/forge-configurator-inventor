using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class MigrationBucketKeyProvider : IBucketKeyProvider
    {
        private string BucketKey = "";
        public string AnonymousBucketKey {get;}

        public Task<string> GetBucketKeyAsync()
        {
            return Task.FromResult(BucketKey);
        }
        public void SetBucketKey(string bucketKey)
        {
            BucketKey = bucketKey;
        }
    }
}
