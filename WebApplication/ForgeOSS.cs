using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using WebApplication.Utilities;

namespace WebApplication
{
    /// <summary>
    /// Class to work with Forge APIs.
    /// </summary>
    public class ForgeOSS : IForgeOSS
    {
        private readonly ILogger<ForgeOSS> _logger;
        private static readonly Scope[] _scope = { Scope.DataRead, Scope.DataWrite, Scope.BucketCreate, Scope.BucketDelete, Scope.BucketRead };

        private readonly RetryPolicy _refreshTokenPolicy;

        public Task<string> TwoLeggedAccessToken => _twoLeggedAccessToken.Value;
        private Lazy<Task<string>> _twoLeggedAccessToken;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ForgeOSS(IOptions<ForgeConfiguration> optionsAccessor, ILogger<ForgeOSS> logger)
        {
            _logger = logger;
            Configuration = optionsAccessor.Value.Validate();

            RefreshApiToken();

            // create policy to refresh API token on expiration (401 error code)
            _refreshTokenPolicy = Policy
                                    .Handle<ApiException>(e => e.ErrorCode == StatusCodes.Status401Unauthorized)
                                    .RetryAsync(5, (_, __) => RefreshApiToken());
        }

        private async Task<string> _2leggedAsync()
        {
            _logger.LogInformation("Refreshing Forge token");

            // Call the asynchronous version of the 2-legged client with HTTP information
            // HTTP information helps to verify if the call was successful as well as read the HTTP transaction headers.
            var twoLeggedApi = new TwoLeggedApi();
            Autodesk.Forge.Client.ApiResponse<dynamic> response = await twoLeggedApi.AuthenticateAsyncWithHttpInfo(Configuration.ClientId, Configuration.ClientSecret, oAuthConstants.CLIENT_CREDENTIALS, _scope);
            if (response.StatusCode != StatusCodes.Status200OK)
            {
                throw new Exception("Request failed! (with HTTP response " + response.StatusCode + ")");
            }

            // The JSON response from the oAuth server is the Data variable and has already been parsed into a DynamicDictionary object.
            dynamic bearer = response.Data;
            return bearer.access_token;
        }

        public async Task<List<ObjectDetails>> GetBucketObjectsAsync(string bucketKey, string beginsWith = null)
        {
            return await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var objects = new List<ObjectDetails>();

                ObjectsApi objectsApi = await GetObjectsApiAsync();
                dynamic objectsList = await objectsApi.GetObjectsAsync(bucketKey, null, beginsWith);

                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
                {
                    var details = new ObjectDetails
                    {
                        BucketKey = objInfo.Value.bucketKey,
                        ObjectId = objInfo.Value.objectId,
                        ObjectKey = objInfo.Value.objectKey,
                        Sha1 = Encoding.ASCII.GetBytes(objInfo.Value.sha1),
                        Size = (int?) objInfo.Value.size,
                        Location = objInfo.Value.location
                    };
                    objects.Add(details);
                }

                return objects;
            });
        }


        /// <summary>
        /// Create bucket with given name
        /// </summary>
        /// <param name="bucketKey">The bucket name.</param>
        public async Task CreateBucketAsync(string bucketKey)
        {
            var payload = new PostBucketsPayload(bucketKey, /*allow*/null, PostBucketsPayload.PolicyKeyEnum.Persistent);

            await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = await GetBucketsApiAsync();
                await api.CreateBucketAsync(payload, /* use default (US region) */ null);
            });
        }

        public async Task DeleteBucketAsync(string bucketKey)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = await GetBucketsApiAsync();
                await api.DeleteBucketAsync(bucketKey);
            });
        }

        public async Task CreateEmptyObjectAsync(string bucketKey, string objectName)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = await GetObjectsApiAsync();
                await using var stream = new MemoryStream();
                await api.UploadObjectAsync(bucketKey, objectName, 0, stream);
            });
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
        public async Task<string> CreateSignedUrlAsync(string bucketKey, string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            var signature = new PostBucketsSigned(minutesExpiration);

            return await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = await GetObjectsApiAsync();
                dynamic result = await api.CreateSignedResourceAsync(bucketKey, objectName, signature, AsString(access));
                return result.signedUrl;
            });
        }

        public async Task UploadObjectAsync(string bucketKey, string objectName, Stream stream)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                ObjectsApi objectsApi = await GetObjectsApiAsync();
                await objectsApi.UploadObjectAsync(bucketKey, objectName, 0, stream);
            });
        }

        public async Task UploadChunkAsync(string bucketKey, Stream stream, string objectName, string contentRange, string sessionId)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = await GetObjectsApiAsync();
                await api.UploadChunkAsync(bucketKey, objectName, (int) stream.Length, contentRange, sessionId, stream);
            });
        }
        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObjectAsync(string bucketKey, string oldName, string newName)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                // OSS does not support renaming, so emulate it with more ineffective operations
                var api = await GetObjectsApiAsync();
                await api.CopyToAsync(bucketKey, oldName, newName);
                await api.DeleteObjectAsync(bucketKey, oldName);
            });
        }

        public async Task<Autodesk.Forge.Client.ApiResponse<dynamic>> GetObjectAsync(string bucketKey, string objectName)
        {
            return await _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = await GetObjectsApiAsync();
                return await api.GetObjectAsyncWithHttpInfo(bucketKey, objectName);
            });
        }

        private async Task<ObjectsApi> GetObjectsApiAsync()
        {
            return new ObjectsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
        }

        private async Task<BucketsApi> GetBucketsApiAsync()
        {
            return new BucketsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
        }

        private static string AsString(ObjectAccess access)
        {
            return access.ToString().ToLowerInvariant();
        }

        private void RefreshApiToken()
        {
            _twoLeggedAccessToken = new Lazy<Task<string>>(async () => await _2leggedAsync());
        }
    }
}
