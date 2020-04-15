using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;

namespace WebApplication
{
    public interface IForge
    {
        /// <summary>
        /// Forge configuration.
        /// </summary>
        ForgeConfiguration Configuration { get; }

        Task<List<ObjectDetails>> GetBucketObjects(string bucketKey, string beginsWith = null);
        Task CreateBucket(string name);
        Task DeleteBucket(string name);
        Task CreateEmptyObject(string bucketKey, string objectName);
        Task UploadObject(string bucketKey, Stream stream, string objectName);

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <returns>Signed URL</returns>
        Task<string> CreateSignedUrl(string bucketKey, string objectName);
    }
}
