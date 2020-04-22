using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.Utilities;

namespace WebApplication
{
    /// <summary>
    /// Logic related to local cache storage.
    /// </summary>
    public class LocalStorage
    {
        private const string LocalCacheDir = "LocalCache";

        /// <summary>
        /// Full pathname to the dir with cache.
        /// </summary>
        public string LocalDir { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalStorage(string rootDir)
        {
            if (string.IsNullOrEmpty(rootDir)) throw new ArgumentNullException(nameof(rootDir));

            LocalDir = Path.Combine(rootDir, LocalCacheDir);

            // ensure the directory is exists
            Directory.CreateDirectory(LocalDir);
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocal(Project project, IForgeOSS forge, HttpClient httpClient, string bucketKey)
        {
            var thumbnailUrl = await forge.CreateSignedUrlAsync(bucketKey, project.Attributes.Thumbnail);
            var localFile = Path.Combine(LocalDir, project.Attributes.Thumbnail);
            await httpClient.DownloadAsync(thumbnailUrl, localFile);
        }
    }
}
