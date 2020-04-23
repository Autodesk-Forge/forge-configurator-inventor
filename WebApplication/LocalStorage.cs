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
        private readonly Project _project;
        private readonly ResourceProvider _resourceProvider;
        private readonly string _baseDir;
        private readonly INameProvider _localNames;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalStorage(Project project, ResourceProvider resourceProvider)
        {
            _project = project;
            _resourceProvider = resourceProvider;

            _baseDir = Path.Combine(resourceProvider.LocalRootName, _project.Name);
            Directory.CreateDirectory(_baseDir);

            _localNames = _project.LocalNames(_baseDir);
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(HttpClient httpClient)
        {
            // get thumbnail
            await DownloadFileAsync(httpClient, _project.Attributes, LocalName.Thumbnail);

            // get metadata and extract project attributes
            var metadataFile = await DownloadFileAsync(httpClient, _project.Attributes, LocalName.Metadata);

            var fileContent = await File.ReadAllTextAsync(metadataFile, Encoding.UTF8);
            var projectAttributes = JsonSerializer.Deserialize<ProjectAttributes>(fileContent);

            // download ZIP with SVF model
            var keyProvider = _project.KeyProvider(projectAttributes.Hash);
            var svfModelZip = await DownloadFileAsync(httpClient, keyProvider, LocalName.ModelView);

            // extract SVF from the archive
            var svfDir = Path.Combine(_baseDir, "svf", projectAttributes.Hash);
            ZipFile.ExtractToDirectory(svfModelZip, svfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            File.Delete(svfModelZip);

            // TODO: write marker file
        }

        private async Task<string> DownloadFileAsync(HttpClient httpClient, INameProvider nameProvider, string localFile)
        {
            // generate filename for destination file
            string localFullName = _localNames.ToFullName(localFile);

            // generate signed URL to the OSS object
            string url = await _resourceProvider.CreateSignedUrlAsync(nameProvider.ToFullName(localFile));

            // and download the file
            await httpClient.DownloadAsync(url, localFullName);
            return localFullName;
        }
    }
}
