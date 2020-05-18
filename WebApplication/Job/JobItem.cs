using System;

namespace WebApplication.Job
{
    public class JobItem
    {
        public string ProjectId { get; }
        public string Data { get; }
        public string Id { get; }

        public JobItem(string projectId, string data)
        {
            this.ProjectId = projectId;
            this.Data = data;
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
