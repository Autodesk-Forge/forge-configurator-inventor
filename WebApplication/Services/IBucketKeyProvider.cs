using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IBucketKeyProvider
    {
        Task<string> GetBucketKeyAsync();
    }
}
