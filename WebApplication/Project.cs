using System;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Project
    {
        public Project(string projectName) 
        {
            if(Name == string.Empty)
            {
                throw new ArgumentException("Initializing Project with empty name", nameof(projectName));
            }

            Name = projectName; 

            OSSSourceModel = $"{ONC.ProjectsFolder}-{projectName}";
            OSSThumbnail = $"{ONC.CacheFolder}-{projectName}-original-thumbnail.png";
            HrefThumbnail = "bike.png"; // temporary icon
        }

        public static Project FromObjectKey(string objectKey)
        {
            if(!objectKey.StartsWith($"{ONC.ProjectsFolder}-"))
            {
                throw new Exception("Initializing Project from invalid bucket key: " + objectKey);
            }

            var projectName = objectKey.Substring(ONC.ProjectsFolder.Length+1);
            return new Project(projectName);
        }

        public string Name { get; }
        public string OSSSourceModel {get; }
        public string OSSThumbnail { get; }
        public string OriginalSvfZip => $"{ONC.CacheFolder}-{Name}-model.zip"; // TODO: ???
        public string ParametersJson => $"{ONC.CacheFolder}-{Name}-parameters.json"; // TODO: ???
        public string HrefThumbnail { get; }

        public OSSObjectKeyProvider KeyProvider(string hash) => new OSSObjectKeyProvider(Name, hash);
    }
}