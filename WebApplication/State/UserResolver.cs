using System.Threading.Tasks;
using WebApplication.Services;
using WebApplication.Utilities;

namespace WebApplication.State
{
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

        public async Task<string> GetBucketKey()
        {
            if (IsAuthenticated)
            {
                var profile = await _forgeOSS.GetProfileAsync(Token);
                var userId = profile.userId;

                var userHash = Crypto.GenerateHashString(userId);

                var bucketKey = $"authd-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();
                await _forgeOSS.CreateBucketAsync(bucketKey); // TODO: can throw an exception?

                return bucketKey;
            }
            else
            {
                return _resourceProvider.BucketKey;
            }
        }
    }
}