using System.Collections.Generic;
using System.IO;
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
        public string BucketKey { get; }
        private readonly IForgeOSS _forgeOSS;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="forgeOSS">Forge OSS service.</param>
        /// <param name="bucketKey">The bucket key.</param>
        public OssBucket(IForgeOSS forgeOSS, string bucketKey)
        {
            BucketKey = bucketKey;
            _forgeOSS = forgeOSS;
        }

        /// <summary>
        /// Create bucket.
        /// </summary>
        public async Task CreateAsync()
        {
            await _forgeOSS.CreateBucketAsync(BucketKey);
        }

        /// <summary>
        /// Delete the bucket.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await _forgeOSS.DeleteBucketAsync(BucketKey);
        }

        /// <summary>
        /// Delete the buckets.
        /// </summary>
        /// <param name="buckets">List of backet names</param>
        public async Task DeleteBucketsAsync(List<string> buckets)
        {
            foreach(string bucketName in buckets)
                await _forgeOSS.DeleteBucketAsync(bucketName);
        }

        /// <summary>
        /// Get bucket objects.
        /// </summary>
        /// <param name="beginsWith">Search filter ("begin with")</param>
        public async Task<List<ObjectDetails>> GetObjectsAsync(string beginsWith = null)
        {
            return await _forgeOSS.GetBucketObjectsAsync(BucketKey, beginsWith);
        }

        /// <summary>
        /// List all buckets.
        /// </summary>
        /// <returns>List of buckets</returns>
        public async Task<List<string>> GetBucketsAsync()
        {
            return await _forgeOSS.GetBucketsAsync();
        }

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public async Task<string> CreateSignedUrlAsync(string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            return await _forgeOSS.CreateSignedUrlAsync(BucketKey, objectName, access, minutesExpiration);
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public async Task CopyAsync(string fromName, string toName)
        {
            await _forgeOSS.CopyAsync(BucketKey, fromName, toName);
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public async Task DownloadFileAsync(string objectName, string localFullName)
        {
            await _forgeOSS.DownloadFileAsync(BucketKey, objectName, localFullName);
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObjectAsync(string oldName, string newName)
        {
            await _forgeOSS.RenameObjectAsync(BucketKey, oldName, newName);
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public async Task DeleteObjectAsync(string objectName)
        {
            await _forgeOSS.DeleteAsync(BucketKey, objectName);
        }

        public async Task UploadObjectAsync(string objectName, Stream stream)
        {
            await _forgeOSS.UploadObjectAsync(BucketKey, objectName, stream);
        }

        public async Task UploadChunkAsync(string objectName, string contentRange, string sessionId, Stream stream)
        {
            // public async Task UploadChunkAsync(string bucketKey, )
            await _forgeOSS.UploadChunkAsync(BucketKey, objectName, contentRange, sessionId, stream);
        }

        public async Task<Autodesk.Forge.Client.ApiResponse<dynamic>> GetObjectAsync(string objectName)
        {
            return await _forgeOSS.GetObjectAsync(BucketKey, objectName);
        }
    }
}
