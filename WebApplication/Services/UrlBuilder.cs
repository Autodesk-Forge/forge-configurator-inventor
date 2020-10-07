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

        public string GetUpdateCallbackUrl(string clientId, string hash, string projectId, string arrangerPrefix, string jobId)
        {
            return string.Format(_callbackUrlsConfiguration.Methods.Update, _callbackUrlsConfiguration.Base, 
                clientId, hash, projectId, arrangerPrefix, jobId);
        }


        public string GetAdoptCallbackUrl(string clientId, string projectId, string topLevelAssembly,
            string arrangerPrefix,  string jobId)
        {
            return string.Format(_callbackUrlsConfiguration.Methods.Update, _callbackUrlsConfiguration.Base,
                clientId, projectId, topLevelAssembly, arrangerPrefix, jobId);
        }

        public string GetGenerateSatCallbackUrl(string clientId, string projectId, string hash,
            string arrangerPrefix, string jobId, string satUrl)
        {
            return string.Format(_callbackUrlsConfiguration.Methods.GenSat, _callbackUrlsConfiguration.Base,
                clientId, projectId, hash, arrangerPrefix, jobId, satUrl);
        }

        public string GetGenerateRfaCallbackUrl(string clientId, string projectId, string hash,
            string arrangerPrefix, string jobId, string statistics)
        {
            return string.Format(_callbackUrlsConfiguration.Methods.GenRfa, _callbackUrlsConfiguration.Base,
                clientId, projectId, hash, arrangerPrefix, jobId, statistics);
        }
    }
}