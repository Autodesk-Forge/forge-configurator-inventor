using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Utilities;

namespace WebApplication
{
    /// <summary>
    /// Class to work with Forge APIs.
    /// </summary>
    class Forge : IForge
    {
        private readonly ILogger<Forge> _logger;
        private static readonly Scope[] _scope = { Scope.DataRead, Scope.DataWrite, Scope.BucketCreate, Scope.BucketDelete, Scope.BucketRead };

        // Initialize the 2-legged oAuth 2.0 client.
        private static readonly TwoLeggedApi _twoLeggedApi = new TwoLeggedApi();

        private string _twoLeggedAccessToken;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="optionsAccessor"></param>
        public Forge(IOptions<ForgeConfiguration> optionsAccessor, ILogger<Forge> logger)
        {
            _logger = logger;
            Configuration = optionsAccessor.Value.Validate();
        }

        private async Task<string> GetTwoLeggedAccessToken()
        {
            if (_twoLeggedAccessToken == null)
            {
                dynamic bearer = await _2leggedAsync();
                _twoLeggedAccessToken = bearer.access_token;
            }

            return _twoLeggedAccessToken;
        }

        private async Task<dynamic> _2leggedAsync()
        {
            // Call the asynchronous version of the 2-legged client with HTTP information
            // HTTP information helps to verify if the call was successful as well as read the HTTP transaction headers.
            Autodesk.Forge.Client.ApiResponse<dynamic> response = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(Configuration.ClientId, Configuration.ClientSecret, oAuthConstants.CLIENT_CREDENTIALS, _scope);

            if (response.StatusCode != StatusCodes.Status200OK)
            {
                throw new Exception("Request failed! (with HTTP response " + response.StatusCode + ")");
            }

            // The JSON response from the oAuth server is the Data variable and has already been parsed into a DynamicDictionary object.
            return response.Data;
        }

        public async Task<List<ObjectDetails>> GetBucketObjects(string bucketKey, string beginsWith = null)
        {
            ObjectsApi objectsApi = new ObjectsApi{ Configuration = { AccessToken = await GetTwoLeggedAccessToken() }};

            var objects = new List<ObjectDetails>();

            dynamic objectsList = await objectsApi.GetObjectsAsync(bucketKey, null, beginsWith);
            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
            {
                var details = new ObjectDetails
                {
                    BucketKey = objInfo.Value.bucketKey,
                    ObjectId = objInfo.Value.objectId,
                    ObjectKey = objInfo.Value.objectKey,
                    Sha1 = System.Text.Encoding.ASCII.GetBytes(objInfo.Value.sha1),
                    Size = (int?)objInfo.Value.size,
                    Location = objInfo.Value.location
                };
                objects.Add(details);
            }

            return objects;
        }

        /// <summary>
        /// Create bucket with given name
        /// </summary>
        /// <param name="bucketName">The bucket name.</param>
        public async Task CreateBucket(string bucketName)
        {
            var api = new BucketsApi { Configuration = { AccessToken = await GetTwoLeggedAccessToken() }};

            var payload = new PostBucketsPayload(bucketName, /*allow*/null, PostBucketsPayload.PolicyKeyEnum.Persistent);
            await api.CreateBucketAsync(payload, /* use default (US region) */ null);
        }

        public async Task DeleteBucket(string bucketName)
        {
            var api = new BucketsApi { Configuration = { AccessToken = await GetTwoLeggedAccessToken() }};
            await api.DeleteBucketAsync(bucketName);
        }

        public async Task CreateEmptyObject(string bucketKey, string objectName)
        {
            await CreateEmptyObjectInner(bucketKey, objectName, await GetObjectsApi());
        }

        /// <summary>
        /// Create an empty object at OSS and generate a signed URL to it.
        /// </summary>
        /// <returns>Signed URL</returns>
        public async Task<string> CreateDestinationUrl(string bucketKey, string objectName)
        {
            ObjectsApi objectsApi = await GetObjectsApi();

            // create empty object
            await CreateEmptyObjectInner(bucketKey, objectName, objectsApi);

            // and get URL to it
            dynamic result = await objectsApi.CreateSignedResourceAsync(bucketKey, objectName, new PostBucketsSigned(30), "write");
            return result.signedUrl;
        }

        private static async Task CreateEmptyObjectInner(string bucketKey, string objectName, IObjectsApi objectsApi)
        {
            using(var stream = new MemoryStream())
            {
                await objectsApi.UploadObjectAsync(bucketKey, objectName, 0, stream);
            }
        }

        private async Task<ObjectsApi> GetObjectsApi()
        {
            return new ObjectsApi{ Configuration = { AccessToken = await GetTwoLeggedAccessToken() }}; // TODO: ER: cache? Or is it lightweight operation?
        }

        public async Task UploadObject(string bucketKey, Stream stream, string objectName)
        {
            ObjectsApi objectsApi = new ObjectsApi { Configuration = { AccessToken = await GetTwoLeggedAccessToken() } };

            await objectsApi.UploadObjectAsync(bucketKey, objectName, 0, stream);
        }
    }
}
