using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication.Services
{
    public class MigrationBucketKeyProvider : IBucketKeyProvider
    {
        public string BucketKey { private get; set; }

        public Task<string> GetBucketKeyAsync()
        {
            return new Task<string>(() => BucketKey);
        }
    }
}
