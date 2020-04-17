namespace WebApplication.Utilities
{
    /// <summary>
    /// Object Name Constants
    /// </summary>
    public static class ONC // aka ObjectNameConstants
    {
        public const string ProjectsFolder = "projects";
        public const string CacheFolder = "cache";
        public const string DownloadsFolder = "downloads";
    }

    public class OSSObjectKeyProvider
    {
        private readonly string _hashDir;

        public OSSObjectKeyProvider(string projectName, string parametersHash)
        {
            _hashDir = $"{ONC.CacheFolder}-{projectName}-{parametersHash}";
        }

        /// <summary>
        /// Filename for ZIP with current model state.
        /// </summary>
        public string CurrentModel => ToFullName("model.zip");
        
        /// <summary>
        /// Filename for ZIP with SVF model.
        /// </summary>
        public string ModelView => ToFullName("model-view.zip");

        /// <summary>
        /// Filename for JSON with Inventor document parameters.
        /// </summary>
        public string Parameters => ToFullName("parameters.json");

        public string DownloadsPath => ToFullName(ONC.DownloadsFolder);

        /// <summary>
        /// Generate full relative name for the filename.
        /// </summary>
        public string ToFullName(string fileName)
        {
            return _hashDir + "-" + fileName;
        }
    }
}
