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
        public async Task EnsureLocalAsync(HttpClient httpClient, IForgeOSS forgeOSS)
        {
            // ensure the directory exists
            Directory.CreateDirectory(Project.LocalAttributes.BaseDir);

            // download metadata and thumbnail
            await Task.WhenAll(
                                DownloadFileAsync(httpClient, forgeOSS, Project.OssAttributes.Metadata, Project.LocalAttributes.Metadata),
                                DownloadFileAsync(httpClient, forgeOSS, Project.OssAttributes.Thumbnail, Project.LocalAttributes.Thumbnail)
                            );


            // download ZIP with SVF model
            // NOTE: this step is impossible without having project metadata,
            // because file/dir names depends on hash of initial project state

            await PlaceViewablesAsync(httpClient, forgeOSS, GetLocalNames(), GetOssNames());
        }

        /// <summary>
        /// Ensure the project viewables are cached locally.
        /// </summary>
        /// <param name="httpClient">HTTP client.</param>
        /// <param name="forgeOSS">OSS client</param>
        /// <param name="hash">Parameters hash.</param>
        public Task EnsureViewablesAsync(HttpClient httpClient, IForgeOSS forgeOSS, string hash)
        {
            return PlaceViewablesAsync(httpClient, forgeOSS, GetLocalNames(hash), GetOssNames(hash));
        }

        private async Task PlaceViewablesAsync(HttpClient httpClient, IForgeOSS forgeOSS, LocalNameProvider localNames,
            OSSObjectNameProvider ossNames)
        {
            // create the "hashed" dir
            Directory.CreateDirectory(localNames.BaseDir);

            using var tempFile = new TempFile();
            await DownloadFileAsync(httpClient, forgeOSS, ossNames.ModelView, tempFile.Name);
            await DownloadFileAsync(httpClient, forgeOSS, ossNames.Parameters, localNames.Parameters);

            // extract SVF from the archive
            ZipFile.ExtractToDirectory(tempFile.Name, localNames.SvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported
        }


        /// <summary>
        /// Downloads OSS file locally.
        /// </summary>
        private async Task DownloadFileAsync(HttpClient httpClient, IForgeOSS forgeOSS, string objectName, string localFullName)
        {
            // generate signed URL to the OSS object
            string url = await forgeOSS.CreateSignedUrlAsync(_resourceProvider.BucketKey, objectName);

            // and download the file
            await httpClient.DownloadAsync(url, localFullName); // TODO: do it without http client
        }

        /// <summary>
        /// OSS names for "hashed" files.
        /// </summary>
        public OSSObjectNameProvider GetOssNames(string hash = null) => Project.OssNameProvider(hash ?? Metadata.Hash);

        /// <summary>
        /// Local names for "hashed" files.
        /// </summary>
        public LocalNameProvider GetLocalNames(string hash = null) => Project.LocalNameProvider(hash ?? Metadata.Hash);
    }
}
