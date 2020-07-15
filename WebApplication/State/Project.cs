using System;
using WebApplication.Utilities;

namespace WebApplication.State
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
            OSSSourceModel = ExactOssName(projectName);

            OssAttributes = new OssAttributes(projectName);
            LocalAttributes = new LocalAttributes(rootDir, Name);
        }

        public static string ExactOssName(string projectName) => $"{ONC.ProjectsFolder}/{projectName}";

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
