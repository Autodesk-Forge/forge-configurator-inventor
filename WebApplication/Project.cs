using System;

namespace IoConfigDemo
{
    public class Project
    {
        public Project(string projectName) 
        {
            if(Name == string.Empty) {
                throw new Exception("Initializing Project with empty name");
            }

            Name = projectName; 
            SourceModel = $"{ONC.projectsFolder}-{projectName}";
            Thumbnail = $"{ONC.cacheFolder}-{projectName}-original-thumbnail.svg";
        }

        static public Project FromObjectKey(string objectKey) {
            if(!objectKey.StartsWith($"{ONC.projectsFolder}-")) {
                throw new Exception("Initializing Project from invalid bucket key: " + objectKey);
            }

            var projectName = objectKey.Substring(ONC.projectsFolder.Length+1);
            return new Project(projectName);
        }

        public string Name { get; private set; }
        public string SourceModel {get; private set; }
        public string Thumbnail { get; private set; }

        public OSSObjectKeyProvider KeyProvider(string hash) => new OSSObjectKeyProvider(Name, hash);
    }
}