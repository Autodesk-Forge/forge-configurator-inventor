using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using WebApplication.Services;

namespace WebApplication.State
{
    /// <summary>
    /// Wrapper to work with OSS bucket.
    /// </summary>
    public class OssBucket
    {
        private readonly string _bucketKey;
        private readonly IForgeOSS _forgeOSS;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="forgeOSS">Forge OSS service.</param>
        /// <param name="bucketKey">The bucket key.</param>
        public OssBucket(IForgeOSS forgeOSS, string bucketKey)
        {
            _bucketKey = bucketKey;
            _forgeOSS = forgeOSS;
        }

        /// <summary>
        /// Create bucket.
        /// </summary>
        public Task CreateAsync()
        {
            return _forgeOSS.CreateBucketAsync(_bucketKey);
        }

        /// <summary>
        /// Get bucket objects.
        /// </summary>
        /// <param name="beginsWith">Search filter ("begin with")</param>
        public Task<List<ObjectDetails>> GetObjectsAsync(string beginsWith = null)
        {
            return _forgeOSS.GetBucketObjectsAsync(_bucketKey, beginsWith);
        }
    }
}
