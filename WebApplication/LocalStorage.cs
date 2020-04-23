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
            var thumbnailUrl = await GetUrlAsync(_project.Attributes.Thumbnail);
            await DownloadFileAsync(httpClient, LocalName.Thumbnail, thumbnailUrl);

            // get metadata and extract project attributes
            var metadataUrl = await GetUrlAsync(_project.Attributes.Metadata);
            var metadataFile = await DownloadFileAsync(httpClient, LocalName.Metadata, metadataUrl);

            var fileContent = await File.ReadAllTextAsync(metadataFile, Encoding.UTF8);
            var projectAttributes = JsonSerializer.Deserialize<ProjectAttributes>(fileContent);

            // download ZIP with SVF model
            var keyProvider = _project.KeyProvider(projectAttributes.Hash);
            var svfUrl = await GetUrlAsync(keyProvider.ModelView);
            var svfModelZip = await DownloadFileAsync(httpClient, LocalName.ModelView, svfUrl);

            // extract SVF from the archive
            var svfDir = Path.Combine(_baseDir, "svf", projectAttributes.Hash);
            ZipFile.ExtractToDirectory(svfModelZip, svfDir, overwriteFiles: true); // TODO: non-default encoding is not supported

            File.Delete(svfModelZip);

            // TODO: write marker file
        }

        private Task<string> GetUrlAsync(INameProvider nameProvider, string fileName)
        {
            return _resourceProvider.CreateSignedUrlAsync(nameProvider.ToFullName(fileName));
        }

        private Task<string> GetUrlAsync(string fileName)
        {
            return _resourceProvider.CreateSignedUrlAsync(fileName);
        }

        private async Task<string> DownloadFileAsync(HttpClient httpClient, string localFile, string url)
        {
            var localFullName = _localNames.ToFullName(localFile);
            await httpClient.DownloadAsync(url, localFullName);
            return localFullName;
        }
    }
}
