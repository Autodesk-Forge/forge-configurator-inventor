using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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

        private readonly Lazy<Task<string>> _twoLeggedAccessToken;
        public Task<string> TwoLeggedAccessToken => _twoLeggedAccessToken.Value;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Forge(IOptions<ForgeConfiguration> optionsAccessor, ILogger<Forge> logger)
        {
            _logger = logger;
            Configuration = optionsAccessor.Value.Validate();
            _twoLeggedAccessToken = new Lazy<Task<string>>(async () =>
            {
                return await _2leggedAsync();
            });
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
            dynamic bearer = response.Data;
            return bearer.access_token;
        }

        public async Task<List<ObjectDetails>> GetBucketObjects(string bucketKey, string beginsWith = null)
        {
            ObjectsApi objectsApi = await GetObjectsApi();

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
            var api = new BucketsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };

            var payload = new PostBucketsPayload(bucketName, /*allow*/null, PostBucketsPayload.PolicyKeyEnum.Persistent);
            await api.CreateBucketAsync(payload, /* use default (US region) */ null);
        }

        public async Task DeleteBucket(string bucketName)
        {
            var api = new BucketsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
            await api.DeleteBucketAsync(bucketName);
        }

        public async Task CreateEmptyObject(string bucketKey, string objectName)
        {
            ObjectsApi objectsApi = await GetObjectsApi();

            using (var stream = new MemoryStream())
            {
                await ((IObjectsApi)objectsApi).UploadObjectAsync(bucketKey, objectName, 0, stream);
            }
        }

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public async Task<string> CreateSignedUrl(string bucketKey, string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            ObjectsApi objectsApi = await GetObjectsApi();

            dynamic result = await objectsApi.CreateSignedResourceAsync(bucketKey, objectName, new PostBucketsSigned(minutesExpiration), AsString(access));
            return result.signedUrl;
        }

        public async Task UploadObject(string bucketKey, Stream stream, string objectName)
        {
            ObjectsApi objectsApi = await GetObjectsApi();
            await objectsApi.UploadObjectAsync(bucketKey, objectName, 0, stream);
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObject(string bucketKey, string oldName, string newName)
        {
            // OSS does not support renaming, so emulate it with more ineffective operations
            ObjectsApi objectsApi = await GetObjectsApi();
            await objectsApi.CopyToAsync(bucketKey, oldName, newName);
            await objectsApi.DeleteObjectAsync(bucketKey, oldName);
        }

        private async Task<ObjectsApi> GetObjectsApi()
        {
            return new ObjectsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } }; // TODO: ER: cache? Or is it lightweight operation?
        }

        private static string AsString(ObjectAccess access)
        {
            return access.ToString().ToLowerInvariant();
        }
    }
}
