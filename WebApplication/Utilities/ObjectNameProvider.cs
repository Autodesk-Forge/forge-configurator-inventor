namespace IoConfigDemo
{  
    public static class ONK // aka ObjectNameConstants
    {
        public const string projectsFolder = "projects";
        public const string cacheFolder = "cache";
        public const string downloadsFolder = "downloads";

    }

    public class ObjectNameProvider
    {
       // use constructor taking only project name to access the original files
        public ObjectNameProvider(string projectName) : this(projectName, "original")
        {
            CurrentModel = $"{ONK.projectsFolder}-{projectName}";
        }

        public ObjectNameProvider(string bucketName, bool isBucketName) : this(bucketName)
        {
            if(!(isBucketName || bucketName.StartsWith($"{ONK.projectsFolder}-"))) {
                return;
            }

            ProjectName = bucketName.Substring(ONK.projectsFolder.Length+1);
        }

        // use constructor with parametersHash to access objects from Cache
        public ObjectNameProvider(string projectName, string parametersHash)
        {
            ProjectName = projectName;
            SourceModel = $"{ONK.projectsFolder}-{projectName}";
            CurrentModel = $"{ONK.cacheFolder}-{projectName}-{parametersHash}-model.zip";
            Thumbnail = $"{ONK.cacheFolder}-{projectName}-{parametersHash}-thumbnail.svg";
            ModelView = $"{ONK.cacheFolder}-{projectName}-{parametersHash}-model-view.svf";
            DownloadsPath = $"{ONK.cacheFolder}-{projectName}-{parametersHash}-{ONK.downloadsFolder}";
        }

        // bucket object names
        public string ProjectName { get; }
        public string SourceModel { get; }
        public string CurrentModel { get; }
        public string Thumbnail { get; }
        public string ModelView { get; }

        // bucket object prefixes
        public string DownloadsPath { get; }
    }
}