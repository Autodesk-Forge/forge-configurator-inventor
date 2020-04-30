using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Web-related utilities.
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Download URL to the local file.
        /// </summary>
        public static async Task DownloadAsync(this HttpClient httpClient, string url, string localFile)
        {
            await using var httpStream = await httpClient.GetStreamAsync(url);
            await using var localStream = File.Create(localFile);
            await httpStream.CopyToAsync(localStream);
        }
    }
}
