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

            _lazyMetadata = new Lazy<ProjectMetadata>(() =>
                                                            {
                                                                var metadataFile = _project.LocalAttributes.Metadata;
                                                                return Json.DeserializeFile<ProjectMetadata>(metadataFile);
                                                            });
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(HttpClient httpClient)
        {
            // ensure the directory exists
            Directory.CreateDirectory(_project.LocalAttributes.BaseDir);

            // download metadata and thumbnail
            await Task.WhenAll(
                                DownloadFileAsync(httpClient, _project.OssAttributes.Metadata, _project.LocalAttributes.Metadata),
                                DownloadFileAsync(httpClient, _project.OssAttributes.Thumbnail, _project.LocalAttributes.Thumbnail)
                            );

            // download ZIP with SVF model
            // NOTE: this step is impossible without having project metadata,
            // because file/dir names depends on hash of initial project state
            var ossNames = _project.OssNameProvider(Metadata.Hash);
            using var tempFile = new TempFile();
            await DownloadFileAsync(httpClient, ossNames.ModelView, tempFile.Name);

            // extract SVF from the archive
            var localNames = _project.LocalNameProvider(Metadata.Hash);
            ZipFile.ExtractToDirectory(tempFile.Name, localNames.SvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            // write marker file about processing completion
            await File.WriteAllTextAsync(_project.LocalAttributes.Marker, "done");
        }

        /// <summary>
        /// Downloads OSS file locally.
        /// </summary>
        private async Task DownloadFileAsync(HttpClient httpClient, string objectName, string localFullName)
        {
            // generate signed URL to the OSS object
            string url = await _resourceProvider.CreateSignedUrlAsync(objectName);

            // and download the file
            await httpClient.DownloadAsync(url, localFullName);
        }
    }
}
