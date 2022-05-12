using System.Threading.Tasks;

namespace webapplication.Services
{
    public interface IBucketKeyProvider
    {
        string AnonymousBucketKey {get;}
        Task<string> GetBucketKeyAsync();
    }
}
