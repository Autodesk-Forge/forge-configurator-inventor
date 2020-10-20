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
        private readonly ProfileProvider _profileProvider;
        private readonly IResourceProvider _resourceProvider;
        public string AnonymousBucketKey {get;}

        public LoggedInUserBucketKeyProvider(ProfileProvider profileProvider, IResourceProvider resourceProvider)
        {
            _profileProvider = profileProvider;
            _resourceProvider = resourceProvider;

            AnonymousBucketKey = resourceProvider.BucketKey;
        }

        public async Task<string> GetBucketKeyAsync()
        {
            if (!_profileProvider.IsAuthenticated) return AnonymousBucketKey;

            dynamic profile = await _profileProvider.GetProfileAsync();
            var userId = profile.userId;

            return _resourceProvider.LoggedUserBucketKey(userId);
        }
    }
}
