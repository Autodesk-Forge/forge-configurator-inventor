using System;
using System.Collections.Generic;
using WebApplication.State;

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
        /// JSON file with parameters.
        /// </summary>
        public const string Parameters = "parameters.json";
    }

    /// <summary>
    /// Object Name Constants
    /// </summary>
    public static class ONC // aka ObjectNameConstants
    {
        /// <summary>
        /// Separator to fake directories in OSS filename.
        /// </summary>
        private const string OssSeparator = "/"; // This must stay private

        public const string ProjectsFolder = "projects";
        public const string ShowParametersChanged = "showparameterschanged.json";
        public const string CacheFolder = "cache";
        public const string AttributesFolder = "attributes";

        public static readonly string ProjectsMask = ToPathMask(ProjectsFolder);

        /// <summary>
        /// Extract project name from OSS object name.
        /// </summary>
        /// <param name="ossObjectName">OSS name for the project</param>
        public static string ToProjectName(string ossObjectName)
        {
            if(!ossObjectName.StartsWith(ProjectsMask))
            {
                throw new ApplicationException("Initializing Project from invalid bucket key: " + ossObjectName);
            }

            return ossObjectName.Substring(ProjectsMask.Length);
        }

        /// <summary>
        /// Get collection of OSS search masks for the project.
        /// It allows to get all OSS files related to the project.
        /// </summary>
        /// <remarks>
        /// The collection does NOT include name of the project itself. Use <see cref="Project.ExactOssName"/> to generate it.
        /// </remarks>
        public static IEnumerable<string> ProjectFileMasks(string projectName)
        {
            yield return ToPathMask(AttributesFolder, projectName);
            yield return ToPathMask(CacheFolder, projectName);
        }

        /// <summary>
        /// Join path pieces into OSS name.
        /// </summary>
        public static string Join(params string[] pieces)
        {
            return string.Join(OssSeparator, pieces);
        }

        /// <summary>
        /// Generate OSS name, which serves as a folder mask for subfolders and files.
        /// </summary>
        private static string ToPathMask(params string[] pieces)
        {
            return Join(pieces) + OssSeparator;
        }
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
            return ONC.Join(_namePrefix, fileName);
        }
    }

    /// <summary>
    /// Project owned filenames under "parameters hash" directory at OSS.
    /// </summary>
    public class OSSObjectNameProvider : OssNameConverter
    {
        public OSSObjectNameProvider(string projectName, string parametersHash) :
                base(ONC.Join(ONC.CacheFolder, projectName, parametersHash)) {}

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
        public string Parameters => ToFullName(LocalName.Parameters);

        public string Rfa => ToFullName("result.rfa");
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
        public OssAttributes(string projectName) : base(ONC.Join(ONC.AttributesFolder, projectName)) {}
    }
}
