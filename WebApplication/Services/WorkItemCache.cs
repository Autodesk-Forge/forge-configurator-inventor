using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class WorkItemCacheRecord
    {
        private readonly AutoResetEvent _resetEvent;
        private WorkItemStatus _status;

        public string Id { get; }

        public WorkItemCacheRecord()
        {
            _resetEvent = new AutoResetEvent(false);
        }

        public WorkItemStatus WaitForCompletion()
        {
            _resetEvent.WaitOne();
            return _status;
        }

        public void Complete(WorkItemStatus status)
        {
            _status = status;
            _resetEvent.Set();
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
            return _cache.GetOrAdd(id, (key) => new WorkItemCacheRecord());
        }
    }
}
