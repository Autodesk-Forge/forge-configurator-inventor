using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication
{
    /// <summary>
    /// Logic related to local cache storage.
    /// </summary>
    public class LocalStorage
    {
        private const string LocalCacheDir = "LocalCache";

        private readonly IForgeOSS _forge;
        private readonly ResourceProvider _resourceProvider;

        /// <summary>
        /// Full pathname to the dir with cache.
        /// </summary>
        public string LocalDir { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalStorage(IForgeOSS forge, ResourceProvider resourceProvider)
        {
            _forge = forge;
            _resourceProvider = resourceProvider;

            LocalDir = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir); // TODO: should the root dir be taken from config? or another class?

            // ensure the directory is exists
            Directory.CreateDirectory(LocalDir);
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(Project project, HttpClient httpClient)
        {
            string baseDir = Path.Combine(LocalDir, project.Name);
            Directory.CreateDirectory(baseDir);

            // get thumbnail
            await DownloadFileAsync(httpClient, baseDir, project.Attributes.Thumbnail);

            // get metadata and extract project attributes
            var metadataFile = await DownloadFileAsync(httpClient, baseDir, project.Attributes.Metadata);

            var fileContent = await File.ReadAllTextAsync(metadataFile, Encoding.UTF8);
            var projectAttributes = JsonSerializer.Deserialize<ProjectAttributes>(fileContent);

            // download SVF model
            var paramsHash = projectAttributes.Hash;
            var keyProvider = project.KeyProvider(paramsHash);
            var svfModelZip = await DownloadFileAsync(httpClient, baseDir, keyProvider.ModelView);

            var svfDir = Path.Combine(baseDir, "svf", paramsHash);
            ZipFile.ExtractToDirectory(svfModelZip, svfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            File.Delete(svfModelZip);
        }

        private async Task<string> DownloadFileAsync(HttpClient httpClient, string baseDir, string relativeFile)
        {
            var signedUrl = await _forge.CreateSignedUrlAsync(_resourceProvider.BucketKey, relativeFile);

            var localFile = Path.Combine(baseDir, relativeFile);
            await httpClient.DownloadAsync(signedUrl, localFile);
            return localFile;
        }
    }
}
