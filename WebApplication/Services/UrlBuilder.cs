using Microsoft.Extensions.Options;
using WebApplication.Definitions;

namespace WebApplication.Services
{
    public class UrlBuilder
    {
        private readonly CallbackUrls _callbackUrlsConfiguration;

        public UrlBuilder(IOptions<CallbackUrls> callbackUrlsConfiguration)
        {
            _callbackUrlsConfiguration = callbackUrlsConfiguration.Value;
        }

        public string GetUpdateCallbackUrl(string clientId, string hash, string projectId, string prefix, string jobId)
        {
            return string.Format(_callbackUrlsConfiguration.Methods.Update, _callbackUrlsConfiguration.Base, 
                clientId, hash, projectId, prefix, jobId);
        }
    }
}