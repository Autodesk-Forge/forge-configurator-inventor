using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Model;

namespace IoConfigDemo
{
    public interface IForge
    {
        Task<List<ObjectDetails>> GetBucketObjects(string bucketKey);
    }
}