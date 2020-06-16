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
        public Task CreateAsync()
        {
            return _forgeOSS.CreateBucketAsync(BucketKey);
        }

        /// <summary>
        /// Delete the bucket.
        /// </summary>
        /// <returns></returns>
        public Task DeleteAsync()
        {
            return _forgeOSS.DeleteBucketAsync(BucketKey);
        }

        /// <summary>
        /// Get bucket objects.
        /// </summary>
        /// <param name="beginsWith">Search filter ("begin with")</param>
        public Task<List<ObjectDetails>> GetObjectsAsync(string beginsWith = null)
        {
            return _forgeOSS.GetBucketObjectsAsync(BucketKey, beginsWith);
        }

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public Task<string> CreateSignedUrlAsync(string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            return _forgeOSS.CreateSignedUrlAsync(BucketKey, objectName, access, minutesExpiration);
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public Task CopyAsync(string fromName, string toName)
        {
            return _forgeOSS.CopyAsync(BucketKey, fromName, toName);
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public Task DownloadFileAsync(string objectName, string localFullName)
        {
            return _forgeOSS.DownloadFileAsync(BucketKey, objectName, localFullName);
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public Task RenameObjectAsync(string oldName, string newName)
        {
            return _forgeOSS.RenameObjectAsync(BucketKey, oldName, newName);
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public Task DeleteAsync(string objectName)
        {
            return _forgeOSS.DeleteAsync(BucketKey, objectName);
        }

        public Task UploadObjectAsync(string objectName, Stream stream)
        {
            return _forgeOSS.UploadObjectAsync(BucketKey, objectName, stream);
        }

        public Task<Autodesk.Forge.Client.ApiResponse<dynamic>> GetObjectAsync(string objectName)
        {
            return _forgeOSS.GetObjectAsync(BucketKey, objectName);
        }

    }
}
