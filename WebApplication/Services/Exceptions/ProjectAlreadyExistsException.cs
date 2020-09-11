using System;

namespace WebApplication.Services.Exceptions
{
    public class ProjectAlreadyExistsException : Exception
    {
        private readonly string _projectName;

        public ProjectAlreadyExistsException(string projectName) : base($"Project with name {projectName} already exists")
        {
            _projectName = projectName;
        }
    }
}
