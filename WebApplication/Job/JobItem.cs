using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Job
{
    public class JobItem
    {
        public string ProjectId { get; private set; }
        public string Data { get; private set; }
        public string Id { get; private set; }

        public JobItem(string projectId, string data)
        {
            this.ProjectId = projectId;
            this.Data = data;
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
