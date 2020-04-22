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
        public const string AttributesFolder = "attributes";
    }

    public class BaseNameProvider
    {
        private readonly string _baseDir;

        public BaseNameProvider(string baseDir)
        {
            _baseDir = baseDir;
        }

        /// <summary>
        /// Generate full relative name for the filename.
        /// </summary>
        public string ToFullName(string fileName)
        {
            return _baseDir + "-" + fileName;
        }
    }

    /// <summary>
    /// Project owned filenames under "parameters hash" directory.
    /// </summary>
    public class OSSObjectKeyProvider : BaseNameProvider
    {
        public OSSObjectKeyProvider(string projectName, string parametersHash) : 
                base($"{ONC.CacheFolder}-{projectName}-{parametersHash}") {}

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
    }

    /// <summary>
    /// Project owned filenames in Metadata directory.
    /// </summary>
    public class AttributesNameProvider : BaseNameProvider
    {
        /// <summary>
        /// Filename for thumbnail image.
        /// </summary>
        public string Thumbnail => ToFullName("thumbnail.png");

        /// <summary>
        /// Filename of JSON file with project metadata.
        /// </summary>
        public string Metadata => ToFullName("metadata.json");

        /// <summary>
        /// Constructor.
        /// </summary>
        public AttributesNameProvider(string projectName) : base($"{ONC.AttributesFolder}-{projectName}") {}
    }
}
