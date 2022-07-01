using Autodesk.Forge.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication.Services
{
    public class BucketPrefixProvider
    {
        private readonly ForgeConfiguration _forgeConfig;
        private readonly IConfiguration _configuration;

        public BucketPrefixProvider(IOptions<ForgeConfiguration> forgeConfiguration, IConfiguration configuration)
        {
            _configuration = configuration;
            _forgeConfig = forgeConfiguration.Value;
        }

        public string GetBucketPrefix(string? suffixParam = null)
        {
            var suffix = suffixParam ?? _configuration?.GetValue<string>("BucketKeySuffix");
            return $"authd{suffix}-{_forgeConfig.ClientId}".ToLowerInvariant();
        }
    }
}
