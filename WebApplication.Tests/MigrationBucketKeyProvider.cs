using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication.Services
{
    public class MigrationBucketKeyProvider : LoggedInUserBucketKeyProvider
    {
        public string BucketKey { private get; set; }

        public MigrationBucketKeyProvider(IOptions<ForgeConfiguration> forgeConfiguration, IConfiguration configuration, ProfileProvider profileProvider) : base(forgeConfiguration, configuration, profileProvider)
        {
        }

        public new Task<string> GetBucketKeyAsync()
        {
            return new Task<string>(() => BucketKey);
        }
    }
}
