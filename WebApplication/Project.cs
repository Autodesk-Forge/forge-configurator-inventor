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
            HrefThumbnail = "bike.png"; // temporary icon

            Attributes = new AttributesNameProvider(projectName);
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
        public string HrefThumbnail { get; }

        public OSSObjectKeyProvider KeyProvider(string hash) => new OSSObjectKeyProvider(Name, hash);

        public AttributesNameProvider Attributes { get; }
    }
}
