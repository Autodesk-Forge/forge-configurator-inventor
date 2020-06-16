using System.Threading.Tasks;
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

        public string Token { private get; set; }
        public bool IsAuthenticated => ! string.IsNullOrEmpty(Token);

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS)
        {
            _resourceProvider = resourceProvider;
            _forgeOSS = forgeOSS;
        }

        public async Task<OssBucket> GetBucket()
        {
            if (IsAuthenticated)
            {
                var profile = await _forgeOSS.GetProfileAsync(Token);
                var userId = profile.userId;

                var userHash = Crypto.GenerateHashString(userId);

                var bucketKey = $"authd-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();

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