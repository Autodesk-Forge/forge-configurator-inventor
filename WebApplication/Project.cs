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
            HrefThumbnail = "bike.png"; // temporary icon

            OssAttributes = new OssAttributes(projectName);

            LocalAttributes = new LocalAttributes(rootDir, Name);
        }

        public static Project FromObjectKey(string objectKey, string rootDir)
        {
            if(!objectKey.StartsWith($"{ONC.ProjectsFolder}-"))
            {
                throw new Exception("Initializing Project from invalid bucket key: " + objectKey);
            }

            var projectName = objectKey.Substring(ONC.ProjectsFolder.Length+1);
            return new Project(projectName, rootDir);
        }

        public string Name { get; }
        public string OSSSourceModel { get; }
        public string HrefThumbnail { get; }

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
