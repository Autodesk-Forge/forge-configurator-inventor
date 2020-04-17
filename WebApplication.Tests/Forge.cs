using Autodesk.Forge;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication.Tests
{
    public class Forge
    {
        public DesignAutomationClient FdaClient { get; private set; }
        public ForgeOSS OSSClient { get; private set; }
        public ForgeConfiguration Configuration { get; private set; }

        public Forge()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddForgeAlternativeEnvironmentVariables()
                .Build();

            FdaClient = CreateDesignAutomationClient(configuration);
            Configuration = configuration.GetSection("Forge").Get<ForgeConfiguration>();
            OSSClient = new ForgeOSS(Configuration);
        }

        private DesignAutomationClient CreateDesignAutomationClient(IConfiguration configuration)
        {
            var forgeCfg = configuration.GetSection("Forge").Get<ForgeConfiguration>();
            var httpMessageHandler = new ForgeHandler(Options.Create(forgeCfg))
            {
                InnerHandler = new HttpClientHandler()
            };
            var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
            return new DesignAutomationClient(forgeService);
        }
    }

    public class ForgeOSS
    {
        private ForgeConfiguration _configuration;
        private readonly Lazy<Task<string>> _twoLeggedAccessToken;
        public Task<string> TwoLeggedAccessToken => _twoLeggedAccessToken.Value;
        private static readonly TwoLeggedApi _twoLeggedApi = new TwoLeggedApi();
        private static readonly Scope[] _scope = { Scope.DataRead, Scope.DataWrite, Scope.BucketCreate, Scope.BucketDelete, Scope.BucketRead };

        public ForgeOSS(ForgeConfiguration configuration)
        {
            _configuration = configuration;
            _twoLeggedAccessToken = new Lazy<Task<string>>(async () =>
            {
                return await _2leggedAsync();
            });
        }

        private async Task<dynamic> _2leggedAsync()
        {
            Autodesk.Forge.Client.ApiResponse<dynamic> response = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(_configuration.ClientId, _configuration.ClientSecret, oAuthConstants.CLIENT_CREDENTIALS, _scope);

            if (response.StatusCode != 200)
            {
                throw new Exception("Request failed! (with HTTP response " + response.StatusCode + ")");
            }

            dynamic bearer = response.Data;
            return bearer.access_token;
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            BucketsApi bucketsApi = new BucketsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
            var bucketPayload = new PostBucketsPayload(bucketName, null, PostBucketsPayload.PolicyKeyEnum.Transient);
            await bucketsApi.CreateBucketAsync(bucketPayload, "US");
        }

        public async Task<string> CreateSignedResourceAsync(string bucketKey, string objectName)
        {
            string signedLocation = "";
            ObjectsApi objectsApi = new ObjectsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
            var createOutputSignedResourceResult = await objectsApi.CreateSignedResourceAsyncWithHttpInfo(bucketKey, objectName, new PostBucketsSigned(60), "readwrite");
            signedLocation = createOutputSignedResourceResult.Data.signedUrl;
            return signedLocation;
        }

        public void DeleteBucket(string bucketName)
        {
            BucketsApi bucketsApi = new BucketsApi { Configuration = { AccessToken = TwoLeggedAccessToken.Result } };
            bucketsApi.DeleteBucket(bucketName);
        }
    }
}
