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
        public async Task EnsureLocalAsync(HttpClient httpClient, Project project)
        {
            string baseDir = Path.Combine(LocalDir, project.Name);
            Directory.CreateDirectory(baseDir);

            var localNames = project.LocalNames(baseDir);

            // get thumbnail
            await DownloadFileAsync(httpClient, project.Attributes, localNames, LocalName.Thumbnail);

            // get metadata and extract project attributes
            var metadataFile = await DownloadFileAsync(httpClient, project.Attributes, localNames, LocalName.Metadata);

            var fileContent = await File.ReadAllTextAsync(metadataFile, Encoding.UTF8);
            var projectAttributes = JsonSerializer.Deserialize<ProjectAttributes>(fileContent);

            // download ZIP with SVF model
            var paramsHash = projectAttributes.Hash;
            var keyProvider = project.KeyProvider(paramsHash);
            var svfModelZip = await DownloadFileAsync(httpClient, keyProvider, localNames, LocalName.ModelView);

            // extract SVF from the archive
            var svfDir = Path.Combine(baseDir, "svf", paramsHash);
            ZipFile.ExtractToDirectory(svfModelZip, svfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            File.Delete(svfModelZip);
        }

        private async Task<string> DownloadFileAsync(HttpClient httpClient, INameProvider ossNames, INameProvider localNames, string localFile)
        {
            var signedUrl = await _forge.CreateSignedUrlAsync(_resourceProvider.BucketKey, ossNames.ToFullName(localFile));

            var localFullName = localNames.ToFullName(localFile);
            await httpClient.DownloadAsync(signedUrl, localFullName);
            return localFullName;
        }
    }
}
