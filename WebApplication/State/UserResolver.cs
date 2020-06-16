using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.Utilities;

namespace WebApplication.State
{
    /// <summary>
    /// Business logic to differentiate state for logged in and anonymous users.
    /// </summary>
    public class UserResolver
    {
        private readonly ResourceProvider _resourceProvider;
        private readonly IForgeOSS _forgeOSS;
        private readonly ForgeConfiguration _forgeConfig;

        public string Token { private get; set; }
        public bool IsAuthenticated => ! string.IsNullOrEmpty(Token);

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS, IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _resourceProvider = resourceProvider;
            _forgeOSS = forgeOSS;
            _forgeConfig = forgeConfiguration.Value;
        }

        public async Task<OssBucket> GetBucket()
        {
            if (IsAuthenticated)
            {
                var profile = await _forgeOSS.GetProfileAsync(Token);
                var userId = profile.userId;

                // an OSS bucket must have a unique name, so it should be generated in a way,
                // so it a Forge user gets registered into several deployments it will not cause
                // name collisions. So use client ID (as a salt) to generate bucket name.
                var userHash = Crypto.GenerateHashString(_forgeConfig.ClientId + userId);
                var bucketKey = $"authd-{_forgeConfig.ClientId}-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();

                var bucket = new OssBucket(_forgeOSS, bucketKey);
                await bucket.CreateAsync();

                return bucket;
            }
            else
            {
                return new OssBucket(_forgeOSS, _resourceProvider.BucketKey);
            }
        }
    }
}