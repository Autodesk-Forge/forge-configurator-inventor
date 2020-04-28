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
        /// OSS names for "hashed" files.
        /// </summary>
        public OSSObjectNameProvider OssNames => _lazyOssNames.Value;
        private readonly Lazy<OSSObjectNameProvider> _lazyOssNames;

        /// <summary>
        /// Local names for "hashed" files.
        /// </summary>
        public LocalNameProvider LocalNames => _lazyLocalNames.Value;
        private readonly Lazy<LocalNameProvider> _lazyLocalNames;

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
            _lazyOssNames = new Lazy<OSSObjectNameProvider>(() => _project.OssNameProvider(Metadata.Hash));
            _lazyLocalNames = new Lazy<LocalNameProvider>(() => _project.LocalNameProvider(Metadata.Hash));
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

            // create the "hashed" dir
            Directory.CreateDirectory(LocalNames.BaseDir);

            // download ZIP with SVF model
            // NOTE: this step is impossible without having project metadata,
            // because file/dir names depends on hash of initial project state
            using var tempFile = new TempFile();
            await DownloadFileAsync(httpClient, OssNames.ModelView, tempFile.Name);
            await DownloadFileAsync(httpClient, OssNames.Parameters, LocalNames.Parameters);

            // extract SVF from the archive
            ZipFile.ExtractToDirectory(tempFile.Name, LocalNames.SvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

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

        public ProjectDTO ToDTO()
        {
            return new ProjectDTO
                    {
                        Id = _project.Name,
                        Label = _project.Name,
                        Image = _resourceProvider.ToDataUrl(_project.LocalAttributes.Thumbnail),
                        Svf = _resourceProvider.ToDataUrl(LocalNames.SvfDir)
                    };
        }
    }
}
