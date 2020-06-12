using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        private readonly IHttpClientFactory _clientFactory;
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
        public ForgeOSS(IHttpClientFactory clientFactory, IOptions<ForgeConfiguration> optionsAccessor, ILogger<ForgeOSS> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            Configuration = optionsAccessor.Value.Validate();

            RefreshApiToken();

            // create policy to refresh API token on expiration (401 error code)
            _refreshTokenPolicy = Policy
                                    .Handle<ApiException>(e => e.ErrorCode == StatusCodes.Status401Unauthorized)
                                    .RetryAsync(5, (_, __) => RefreshApiToken());
        }

        public Task<List<ObjectDetails>> GetBucketObjectsAsync(string bucketKey, string beginsWith = null)
        {
            return WithObjectsApiAsync(async api =>
            {
                var objects = new List<ObjectDetails>();

                dynamic objectsList = await api.GetObjectsAsync(bucketKey, null, beginsWith);

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
        public Task CreateBucketAsync(string bucketKey)
        {
            return WithBucketApiAsync(api =>
            {
                var payload = new PostBucketsPayload(bucketKey, /*allow*/null, PostBucketsPayload.PolicyKeyEnum.Persistent);
                return api.CreateBucketAsync(payload, /* use default (US region) */ null);
            });
        }

        public Task DeleteBucketAsync(string bucketKey)
        {
            return WithBucketApiAsync(api => api.DeleteBucketAsync(bucketKey));
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
            return await WithObjectsApiAsync(async api => await GetSignedUrl(api, bucketKey, objectName, access, minutesExpiration));
        }

        public Task UploadObjectAsync(string bucketKey, string objectName, Stream stream)
        {
            return WithObjectsApiAsync(api => api.UploadObjectAsync(bucketKey, objectName, 0, stream));
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public Task RenameObjectAsync(string bucketKey, string oldName, string newName)
        {
            return WithObjectsApiAsync(async api =>
            {
                // OSS does not support renaming, so emulate it with more ineffective operations
                await api.CopyToAsync(bucketKey, oldName, newName);
                await api.DeleteObjectAsync(bucketKey, oldName);
            });
        }

        public Task<Autodesk.Forge.Client.ApiResponse<dynamic>> GetObjectAsync(string bucketKey, string objectName)
        {
            return WithObjectsApiAsync(api => api.GetObjectAsyncWithHttpInfo(bucketKey, objectName));
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public Task CopyAsync(string bucketKey, string fromName, string toName)
        {
            return WithObjectsApiAsync(api => api.CopyToAsync(bucketKey, fromName, toName));
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public Task DeleteAsync(string bucketKey, string objectName)
        {
            return WithObjectsApiAsync(api => api.DeleteObjectAsync(bucketKey, objectName));
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public Task DownloadFileAsync(string bucketKey, string objectName, string localFullName)
        {
            return WithObjectsApiAsync(async api =>
                    {
                        string url = await GetSignedUrl(api, bucketKey, objectName);

                        // and download the file
                        var client = _clientFactory.CreateClient();
                        await client.DownloadAsync(url, localFullName);
                    });
        }

        /// <summary>
        /// Get profile for the user with the access token.
        /// </summary>
        /// <param name="token">Oxygen access token.</param>
        /// <returns>Dynamic object with User Profile</returns>
        /// <remarks>
        /// User Profile fields: https://forge.autodesk.com/en/docs/oauth/v2/reference/http/users-@me-GET/#body-structure-200
        /// </remarks>
        public Task<dynamic> GetProfileAsync(string token)
        {
            var api = new UserProfileApi(new Configuration { AccessToken = token });
            return api.GetUserProfileAsync();
        }

        /// <summary>
        /// Run action against Buckets OSS API.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private Task WithBucketApiAsync(Func<BucketsApi, Task> action)
        {
            return _refreshTokenPolicy.ExecuteAsync(async () =>
                    {
                        var api = new BucketsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
                        return action(api);
                    });
        }

        /// <summary>
        /// Run action against Objects OSS API.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private Task WithObjectsApiAsync(Func<ObjectsApi, Task> action)
        {
            return _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = new ObjectsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
                await action(api);
            });
        }

        /// <summary>
        /// Run action against Objects OSS API.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private Task<T> WithObjectsApiAsync<T>(Func<ObjectsApi, Task<T>> action)
        {
            return _refreshTokenPolicy.ExecuteAsync(async () =>
            {
                var api = new ObjectsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
                return await action(api);
            });
        }

        private static string AsString(ObjectAccess access)
        {
            return access.ToString().ToLowerInvariant();
        }

        private void RefreshApiToken()
        {
            _twoLeggedAccessToken = new Lazy<Task<string>>(async () => await _2leggedAsync());
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

        /// <summary>
        /// Generate signed URL for the OSS object.
        /// </summary>
        private static async Task<string> GetSignedUrl(IObjectsApi api, string bucketKey, string objectName,
                                                            ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            var signature = new PostBucketsSigned(minutesExpiration);
            dynamic result = await api.CreateSignedResourceAsync(bucketKey, objectName, signature, AsString(access));
            return result.signedUrl;
        }
    }
}
