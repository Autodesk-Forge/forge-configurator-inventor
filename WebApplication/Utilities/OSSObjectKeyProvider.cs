namespace IoConfigDemo
{  
    public static class ONC // aka ObjectNameConstants
    {
        public const string projectsFolder = "projects";
        public const string cacheFolder = "cache";
        public const string downloadsFolder = "downloads";

    }

    public class OSSObjectKeyProvider
    {
        public OSSObjectKeyProvider(string projectName, string parametersHash)
        {
            CurrentModel = $"{ONC.cacheFolder}-{projectName}-{parametersHash}-model.zip";
            ModelView = $"{ONC.cacheFolder}-{projectName}-{parametersHash}-model-view.svf";
            DownloadsPath = $"{ONC.cacheFolder}-{projectName}-{parametersHash}-{ONC.downloadsFolder}";
        }

        // bucket object names
        public string CurrentModel { get; }
        public string ModelView { get; }

        // bucket object prefixes
        public string DownloadsPath { get; }
    }
}