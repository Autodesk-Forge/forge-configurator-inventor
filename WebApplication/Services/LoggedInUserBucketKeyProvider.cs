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
        public string AnonymousBucketKey {get;}

        public LoggedInUserBucketKeyProvider(IOptions<ForgeConfiguration> forgeConfiguration, ProfileProvider profileProvider, BucketPrefixProvider bucketPrefixProvider, ResourceProvider resourceProvider)
        {
            _profileProvider = profileProvider;
            _forgeConfig = forgeConfiguration.Value;
            _bucketPrefixProvider = bucketPrefixProvider;

            AnonymousBucketKey = resourceProvider.BucketKey;
        }

        public async Task<string> GetBucketKeyAsync()
        {
            if (!_profileProvider.IsAuthenticated) return AnonymousBucketKey;

            dynamic profile = await _profileProvider.GetProfileAsync();
            var userId = profile.userId;

            // an OSS bucket must have a unique name, so it should be generated in a way,
            // so it a Forge user gets registered into several deployments it will not cause
            // name collisions. So use client ID (as a salt) to generate bucket name.
            var userHash = Crypto.GenerateHashString(_forgeConfig.ClientId + userId);
            var bucketKey = $"{_bucketPrefixProvider.GetBucketPrefix()}-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();

            return bucketKey;
        }

        public void SetBucketKey(string bucketKey)
        {
            throw new NotImplementedException();
        }
    }
}
