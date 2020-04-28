using System;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Project
    {
        public Project(string projectName, string rootDir)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentException("Initializing Project with empty name", nameof(projectName));
            }

            Name = projectName; 
            OSSSourceModel = $"{ONC.ProjectsFolder}-{projectName}";

            OssAttributes = new OssAttributes(projectName);
            LocalAttributes = new LocalAttributes(rootDir, Name);
        }

        public string Name { get; }
        public string OSSSourceModel { get; }

        public OSSObjectNameProvider OssNameProvider(string hash) => new OSSObjectNameProvider(Name, hash);
        public LocalNameProvider LocalNameProvider(string hash) => new LocalNameProvider(LocalAttributes.BaseDir, hash);

        /// <summary>
        /// Full local names for project attribute files.
        /// </summary>
        public LocalAttributes LocalAttributes { get; }

        /// <summary>
        /// Full names for project attributes files (metadata, thumbnails, etc.) at OSS.
        /// </summary>
        public OssAttributes OssAttributes { get; }
    }
}
