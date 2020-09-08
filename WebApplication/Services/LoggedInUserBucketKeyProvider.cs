using System;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public class LoggedInUserBucketKeyProvider : IBucketKeyProvider
    {
        private readonly ForgeConfiguration _forgeConfig;
        private readonly ProfileProvider _profileProvider;
        private readonly BucketPrefixProvider _bucketPrefixProvider;
        private readonly ResourceProvider _resoureProvider;
        public string AnonymousBucketKey {get;}

        public LoggedInUserBucketKeyProvider(IOptions<ForgeConfiguration> forgeConfiguration, ProfileProvider profileProvider, BucketPrefixProvider bucketPrefixProvider, ResourceProvider resourceProvider)
        {
            _profileProvider = profileProvider;
            _forgeConfig = forgeConfiguration.Value;
            _bucketPrefixProvider = bucketPrefixProvider;
            _resoureProvider = resourceProvider;

            AnonymousBucketKey = resourceProvider.BucketKey;
        }

        public async Task<string> GetBucketKeyAsync()
        {
            if (!_profileProvider.IsAuthenticated) return AnonymousBucketKey;

            dynamic profile = await _profileProvider.GetProfileAsync();
            var userId = profile.userId;

            return _resoureProvider.LoggedUserBucketKey(userId);
        }
    }
}
