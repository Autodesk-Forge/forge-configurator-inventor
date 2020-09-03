using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class MigrationBucketKeyProvider : IBucketKeyProvider
    {
        public string BucketKey { private get; set; }

        public Task<string> GetBucketKeyAsync()
        {
            return Task.FromResult(BucketKey);
        }
    }
}
