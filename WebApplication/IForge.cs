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
        /// Create an empty object at OSS and generate a signed URL to it.
        /// </summary>
        /// <returns>Signed URL</returns>
        Task<string> CreateDestinationUrl(string bucketKey, string objectName);
    }
}
