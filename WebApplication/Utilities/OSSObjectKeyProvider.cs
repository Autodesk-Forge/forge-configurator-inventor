using System.IO;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Names for local files.
    /// </summary>
    public static class LocalName
    {
        /// <summary>
        /// Project metadata.
        /// </summary>
        public const string Metadata = "metadata.json";

        /// <summary>
        /// Thumbnail.
        /// </summary>
        public const string Thumbnail = "thumbnail.png";

        /// <summary>
        /// ZIP archive with SVF model.
        /// </summary>
        public const string ModelView = "model-view.zip";

        /// <summary>
        /// Marker file, serves as a flag that project is locally cached.
        /// </summary>
        internal const string Marker = "__marker";
    }

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

    /// <summary>
    /// Convert filename to full name.
    /// </summary>
    public interface INameConverter
    {
        /// <summary>
        /// Generate full name for the filename.
        /// </summary>
        public string ToFullName(string fileName);
    }

    /// <summary>
    /// OSS does not support directories, so emulate folders with long file names.
    /// </summary>
    public class OssNameConverter : INameConverter
    {
        private readonly string _namePrefix;

        public OssNameConverter(string namePrefix)
        {
            _namePrefix = namePrefix;
        }

        /// <summary>
        /// Generate full OSS name for the filename.
        /// </summary>
        public virtual string ToFullName(string fileName)
        {
            return _namePrefix + "-" + fileName;
        }
    }

    /// <summary>
    /// Convert relative filenames to fullnames.
    /// </summary>
    public class LocalNameConverter : INameConverter
    {
        public string BaseDir { get; }

        public LocalNameConverter(string rootDir, string projectDir)
        {
            BaseDir = Path.Combine(rootDir, projectDir);
        }

        /// <summary>
        /// Generate full local name for the filename.
        /// </summary>
        public virtual string ToFullName(string fileName)
        {
            return Path.Combine(BaseDir, fileName);
        }
    }

    /// <summary>
    /// Project owned filenames under "parameters hash" directory.
    /// </summary>
    public class OSSObjectKeyProvider : OssNameConverter
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
        public string ModelView => ToFullName(LocalName.ModelView);

        /// <summary>
        /// Filename for JSON with Inventor document parameters.
        /// </summary>
        public string Parameters => ToFullName("parameters.json");

        public string DownloadsPath => ToFullName(ONC.DownloadsFolder);
    }

    /// <summary>
    /// Project owned filenames in Attributes directory at OSS.
    /// </summary>
    public class AttributesNameProvider : OssNameConverter
    {
        /// <summary>
        /// Filename for thumbnail image.
        /// </summary>
        public string Thumbnail => ToFullName(LocalName.Thumbnail);

        /// <summary>
        /// Filename of JSON file with project metadata.
        /// </summary>
        public string Metadata => ToFullName(LocalName.Metadata);

        /// <summary>
        /// Constructor.
        /// </summary>
        public AttributesNameProvider(string projectName) : base($"{ONC.AttributesFolder}-{projectName}") {}
    }
}
