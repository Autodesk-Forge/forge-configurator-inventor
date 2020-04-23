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
        private readonly IForgeOSS _forge;
        private readonly Project _project;
        private readonly string _baseDir;
        private readonly INameProvider _localNames;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalStorage(IForgeOSS forge, Project project, string rootDir)
        {
            _forge = forge;
            _project = project;

            _baseDir = Path.Combine(rootDir, _project.Name);
            Directory.CreateDirectory(_baseDir);

            _localNames = _project.LocalNames(_baseDir);
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(HttpClient httpClient, string bucketKey)
        {
            // get thumbnail
            await DownloadFileAsync(httpClient, bucketKey, _project.Attributes, LocalName.Thumbnail);

            // get metadata and extract project attributes
            var metadataFile = await DownloadFileAsync(httpClient, bucketKey, _project.Attributes, LocalName.Metadata);

            var fileContent = await File.ReadAllTextAsync(metadataFile, Encoding.UTF8);
            var projectAttributes = JsonSerializer.Deserialize<ProjectAttributes>(fileContent);

            // download ZIP with SVF model
            var paramsHash = projectAttributes.Hash;
            var keyProvider = _project.KeyProvider(paramsHash);
            var svfModelZip = await DownloadFileAsync(httpClient, bucketKey, keyProvider, LocalName.ModelView);

            // extract SVF from the archive
            var svfDir = Path.Combine(_baseDir, "svf", paramsHash);
            ZipFile.ExtractToDirectory(svfModelZip, svfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            File.Delete(svfModelZip);

            // TODO: write marker file
        }

        private async Task<string> DownloadFileAsync(HttpClient httpClient, string bucketKey, INameProvider ossNames, string localFile)
        {
            var signedUrl = await _forge.CreateSignedUrlAsync(bucketKey, ossNames.ToFullName(localFile));

            var localFullName = _localNames.ToFullName(localFile);
            await httpClient.DownloadAsync(signedUrl, localFullName);
            return localFullName;
        }
    }
}
