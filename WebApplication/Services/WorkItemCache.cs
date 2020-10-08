using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class WorkItemCacheRecord
    {
        private WorkItemStatus _status;
        private bool _finished;

        public string Id { get; }

        public WorkItemCacheRecord(string id)
        {
            _finished = false;
            Id = id;
        }

        public async Task<WorkItemStatus> WaitForCompletion()
        {
            while (!IsFinished())
                await Task.Delay(200);

            return _status;
        }

        public void Complete(WorkItemStatus status)
        {
            lock (this)
            {
                _status = status;
                _finished = true;
            }
        }

        public bool IsFinished()
        {
            bool finished;
            lock (this)
            {
                finished = _finished;
            }

            return finished;
        }
    }

    public class WorkItemCache
    {
        private readonly ConcurrentDictionary<string, WorkItemCacheRecord> _cache;

        public WorkItemCache()
        {
            _cache = new ConcurrentDictionary<string, WorkItemCacheRecord>();
        }

        public WorkItemCacheRecord TakeRecord(string id)
        {
            return _cache.Remove(id, out WorkItemCacheRecord record) ? record : null;
        }

        public WorkItemCacheRecord CreateRecord(string id)
        {
            return _cache.GetOrAdd(id, (key) => new WorkItemCacheRecord(key));
        }
    }
}
