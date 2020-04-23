using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication
{
    /// <summary>
    /// Logic related to local cache storage for project.
    /// </summary>
    public class ProjectStorage
    {
        private readonly Project _project;
        private readonly ResourceProvider _resourceProvider;
        private readonly LocalNameConverter _localNames;

        /// <summary>
        /// Fullname of the marker.
        /// </summary>
        private string MarkerFile => _localNames.ToFullName(LocalName.Marker);

        private string DefaultSvfDir
        {
            get
            {
                var svfDir = _localNames.ToFullName("SVF");
                return Path.Combine(svfDir, Metadata.Hash);
            }
        }

        /// <summary>
        /// Project metadata.
        /// </summary>
        public ProjectMetadata Metadata => _lazyMetadata.Value;
        private readonly Lazy<ProjectMetadata> _lazyMetadata;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProjectStorage(Project project, ResourceProvider resourceProvider)
        {
            _project = project;
            _resourceProvider = resourceProvider;

            _localNames = _project.LocalNames(resourceProvider.LocalRootName);

            _lazyMetadata = new Lazy<ProjectMetadata>(() =>
                                                            {
                                                                var metadataFile = _localNames.ToFullName(LocalName.Metadata);
                                                                return Json.DeserializeFile<ProjectMetadata>(metadataFile);
                                                            });
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(HttpClient httpClient)
        {
            // ensure the directory exists
            Directory.CreateDirectory(_localNames.BaseDir);


            var ossAttributes = _project.Attributes;

            // get metadata
            await DownloadFileAsync(httpClient, ossAttributes, LocalName.Metadata);

            // get thumbnail
            await DownloadFileAsync(httpClient, ossAttributes, LocalName.Thumbnail);

            // download ZIP with SVF model
            // NOTE: this step is impossible without having project metadata,
            // because file/dir names depends on hash of initial project state
            var keyProvider = _project.KeyProvider(Metadata.Hash);
            var svfModelZip = await DownloadFileAsync(httpClient, keyProvider, LocalName.ModelView);

            // extract SVF from the archive
            ZipFile.ExtractToDirectory(svfModelZip, DefaultSvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            File.Delete(svfModelZip);

            // write marker file about processing completion
            await File.WriteAllTextAsync(MarkerFile, "done");
        }

        private async Task<string> DownloadFileAsync(HttpClient httpClient, INameConverter ossNameConverter, string fileName)
        {
            // generate fullname for destination file
            string localFullName = _localNames.ToFullName(fileName);

            // generate signed URL to the OSS object
            string url = await _resourceProvider.CreateSignedUrlAsync(ossNameConverter.ToFullName(fileName));

            // and download the file
            await httpClient.DownloadAsync(url, localFullName);
            return localFullName;
        }

        /// <summary>
        /// Throw an exception if project is not cached locally.
        /// </summary>
        private void VerifyCachedState()
        {
            if (! File.Exists(MarkerFile)) throw new ApplicationException($"Project '{_project.Name}' is not cached.");
        }
    }
}
