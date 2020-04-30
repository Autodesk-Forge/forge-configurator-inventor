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
        /// <summary>
        /// Project names.
        /// </summary>
        public Project Project { get; }

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
            Project = project;
            _resourceProvider = resourceProvider;

            _lazyMetadata = new Lazy<ProjectMetadata>(() =>
                                                            {
                                                                var metadataFile = Project.LocalAttributes.Metadata;
                                                                if (!File.Exists(metadataFile))
                                                                    throw new ApplicationException("Attempt to work with uninitialized project storage.");

                                                                return Json.DeserializeFile<ProjectMetadata>(metadataFile);
                                                            });
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(HttpClient httpClient)
        {
            // ensure the directory exists
            Directory.CreateDirectory(Project.LocalAttributes.BaseDir);

            // download metadata and thumbnail
            await Task.WhenAll(
                                DownloadFileAsync(httpClient, Project.OssAttributes.Metadata, Project.LocalAttributes.Metadata),
                                DownloadFileAsync(httpClient, Project.OssAttributes.Thumbnail, Project.LocalAttributes.Thumbnail)
                            );

            // create the "hashed" dir
            var localNames = GetLocalNames();
            Directory.CreateDirectory(localNames.BaseDir);

            // download ZIP with SVF model
            // NOTE: this step is impossible without having project metadata,
            // because file/dir names depends on hash of initial project state
            var ossNames = GetOssNames();
 
            using var tempFile = new TempFile();
            await DownloadFileAsync(httpClient, ossNames.ModelView, tempFile.Name);
            await DownloadFileAsync(httpClient, ossNames.Parameters, localNames.Parameters);

            // extract SVF from the archive
            ZipFile.ExtractToDirectory(tempFile.Name, localNames.SvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            // write marker file about processing completion
            await File.WriteAllTextAsync(Project.LocalAttributes.Marker, "done");
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

        /// <summary>
        /// OSS names for "hashed" files.
        /// </summary>
        public OSSObjectNameProvider GetOssNames() => Project.OssNameProvider(Metadata.Hash);

        /// <summary>
        /// Local names for "hashed" files.
        /// </summary>
        public LocalNameProvider GetLocalNames() => Project.LocalNameProvider(Metadata.Hash);
    }
}
