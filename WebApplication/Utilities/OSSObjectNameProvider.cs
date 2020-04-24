using System.IO;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Names for local files.
    /// </summary>
    internal static class LocalName
    {
        public const string SvfDir = "SVF";

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
        public const string Marker = "__marker";
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
    /// OSS does not support directories, so emulate folders with long file names.
    /// </summary>
    public class OssNameConverter
    {
        private readonly string _namePrefix;

        public OssNameConverter(string namePrefix)
        {
            _namePrefix = namePrefix;
        }

        /// <summary>
        /// Generate full OSS name for the filename.
        /// </summary>
        protected string ToFullName(string fileName)
        {
            return _namePrefix + "-" + fileName;
        }
    }

    /// <summary>
    /// Project owned filenames under "parameters hash" directory at OSS.
    /// </summary>
    public class OSSObjectNameProvider : OssNameConverter
    {
        public OSSObjectNameProvider(string projectName, string parametersHash) : 
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
    public class OssAttributes : OssNameConverter
    {
        /// <summary>
        /// Filename for thumbnail image.`
        /// </summary>
        public string Thumbnail => ToFullName(LocalName.Thumbnail);

        /// <summary>
        /// Filename of JSON file with project metadata.
        /// </summary>
        public string Metadata => ToFullName(LocalName.Metadata);

        /// <summary>
        /// Constructor.
        /// </summary>
        public OssAttributes(string projectName) : base($"{ONC.AttributesFolder}-{projectName}") {}
    }

    /// <summary>
    /// Convert relative filenames to fullnames.
    /// </summary>
    public class LocalNameConverter
    {
        public string BaseDir { get; }

        public LocalNameConverter(string baseDir)
        {
            BaseDir = baseDir;
        }

        /// <summary>
        /// Generate full local name for the filename.
        /// </summary>
        protected string ToFullName(string fileName)
        {
            return Path.Combine(BaseDir, fileName);
        }
    }

    /// <summary>
    /// Local attribute files.
    /// </summary>
    public class LocalAttributes : LocalNameConverter
    {
        /// <summary>
        /// Filename for thumbnail image.
        /// </summary>
        public string Thumbnail => ToFullName(LocalName.Thumbnail);

        /// <summary>
        /// Filename of JSON file with project metadata.
        /// </summary>
        public string Metadata => ToFullName(LocalName.Metadata);

        public string Marker => ToFullName(LocalName.Marker);

        public LocalAttributes(string rootDir, string projectDir) : base(Path.Combine(rootDir, projectDir))
        {
        }
    }

    public class LocalNameProvider : LocalNameConverter
    {
        /// <summary>
        /// Fullname of directory with SVF data.
        /// </summary>
        public string SvfDir => ToFullName(LocalName.SvfDir);

        public LocalNameProvider(string projectDir, string hash) : base(Path.Combine(projectDir, hash))
        {
        }
    }
}
