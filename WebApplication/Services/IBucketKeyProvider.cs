using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IBucketKeyProvider
    {
        string AnonymousBucketKey {get;}
        Task<string> GetBucketKeyAsync();
        void SetBucketKey(string bucketKey);
    }
}
