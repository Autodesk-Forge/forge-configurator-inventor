using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using System.Threading.Tasks;
using WebApplication.Services;

namespace WebApplication.Processing
{
    public interface IWorkItemHandle
    {
        public Task<WorkItemStatus> ProcessWorkItemAsync(WorkItem workItem);
    }

    public class PolledWorkItemHandle : IWorkItemHandle
    {
        private readonly DesignAutomationClient _client;

        public PolledWorkItemHandle(DesignAutomationClient client)
        {
            _client = client;
        }

        public async Task<WorkItemStatus> ProcessWorkItemAsync(WorkItem workItem)
        {
            // run WI and wait for completion
            WorkItemStatus status = await _client.CreateWorkItemAsync(workItem);
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                await Task.Delay(2000);
                status = await _client.GetWorkitemStatusAsync(status.Id);
            }

            return status;
        }
    }

    public class CallbackWorkItemHandle : IWorkItemHandle
    {
        private readonly DesignAutomationClient _client;
        private readonly WorkItemCache _wiCache;

        public CallbackWorkItemHandle(WorkItemCache wiCache, DesignAutomationClient client)
        {
            _client = client;
            _wiCache = wiCache;
        }

        public async Task<WorkItemStatus> ProcessWorkItemAsync(WorkItem workItem)
        {
            // run WI and wait for completion
            WorkItemStatus status = await _client.CreateWorkItemAsync(workItem);
            var record = _wiCache.CreateRecord(status.Id);
            status = record.WaitForCompletion();
            return status;
        }
    }
}
