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

    public interface INameProvider
    {
        /// <summary>
        /// Generate full name for the filename.
        /// </summary>
        public string ToFullName(string fileName);
    }

    /// <summary>
    /// OSS does not support directories, so emulate them with long file OssNameProviderBasenames.
    /// </summary>
    public class OssNameProvider : INameProvider
    {
        private readonly string _namePrefix;

        public OssNameProvider(string namePrefix)
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

    public class LocalNameProvider : INameProvider
    {
        private readonly string _baseDir;

        public LocalNameProvider(string baseDir)
        {
            _baseDir = baseDir;
        }

        /// <summary>
        /// Generate full local name for the filename.
        /// </summary>
        public virtual string ToFullName(string fileName)
        {
            return Path.Combine(_baseDir, fileName);
        }
    }

    /// <summary>
    /// Project owned filenames under "parameters hash" directory.
    /// </summary>
    public class OSSObjectKeyProvider : OssNameProvider
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
    public class AttributesNameProvider : OssNameProvider
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
