using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;

namespace IoConfigDemo
{
    public interface IForge
    {
        Task<List<ObjectDetails>> GetBucketObjects(string bucketKey);
        Task CreateBucket(string name);
        Task DeleteBucket(string name);
        Task CreateEmptyObject(string bucketKey, string objectName);
        /// <summary>
        /// Forge configuration.
        /// </summary>
        ForgeConfiguration Configuration { get; }
    }
}
