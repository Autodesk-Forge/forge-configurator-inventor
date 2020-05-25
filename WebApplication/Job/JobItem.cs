using System;
using WebApplication.Definitions;

namespace WebApplication.Job
{
    public class JobItem
    {
        public string ProjectId { get; }
        public InventorParameters Parameters { get; }
        public string Id { get; }

        public JobItem(string projectId, InventorParameters parameters)
        {
            this.ProjectId = projectId;
            this.Parameters = parameters;
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
