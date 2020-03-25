using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;

namespace IoConfigDemo
{
    public interface IForge
    {
        Task<List<ObjectDetails>> GetBucketObjects(string bucketKey);

        /// <summary>
        /// Forge configuration.
        /// </summary>
        ForgeConfiguration Configuration { get; }
    }
}
