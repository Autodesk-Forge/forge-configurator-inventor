using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Retry;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    /// <summary>
    /// Class to work with Forge APIs.
    /// </summary>
    public class ForgeOSS : IForgeOSS
    {
        /// <summary>
        /// Page size for "Get Bucket Objects" operation.
        /// </summary>
        private const int PageSize = 50;

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

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict) {
                return dict.ContainsKey(name);
            }

            var property = obj.GetType().GetProperty(name);
            return property != null;
        }

        public Task<List<ObjectDetails>> GetBucketObjectsAsync(string bucketKey, string beginsWith = null)
        {
            return WithObjectsApiAsync(async api =>
            {
                var objects = new List<ObjectDetails>();
                string startAt = null; // next page pointer

                do
                {
                    DynamicJsonResponse response = await api.GetObjectsAsync(bucketKey, PageSize, beginsWith, startAt);

                    foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems((response as dynamic).items))
                    {
                        dynamic item = objInfo.Value;

                        var details = new ObjectDetails
                        {
                            BucketKey = item.bucketKey,
                            ObjectId = item.objectId,
                            ObjectKey = item.objectKey,
                            Sha1 = Encoding.ASCII.GetBytes(item.sha1),
                            Size = (int?) item.size,
                            Location = item.location
                        };
                        objects.Add(details);
                    }

                    startAt = null;

                    // check if there is a next page with projects
                    if (response.Dictionary.TryGetValue("next", out var nextPage))
                    {
                        string nextPageUrl = (string) nextPage;
                        if (! string.IsNullOrEmpty(nextPageUrl))
                        {
                            Uri nextUri = new Uri(nextPageUrl, UriKind.Absolute);
                            Dictionary<string, StringValues> query = QueryHelpers.ParseNullableQuery(nextUri.Query);
                            startAt = query["startAt"];
                        }
                    }

                } while (startAt != null);

                return objects;
            });
        }

        /// <summary>
        /// List all buckets
        /// </summary>
        /// <returns>List of all buckets for the account</returns>
        public async Task<List<string>> GetBucketsAsync()
        {
            var buckets = new List<string>();
            await WithBucketApiAsync(async api =>
            {
                dynamic bucketList = await api.GetBucketsAsync(/* use default (US region) */ null);
                foreach (KeyValuePair<string, dynamic> bucketInfo in new DynamicDictionaryItems(bucketList.items))
                {
                    buckets.Add(bucketInfo.Value.bucketKey);
                }
            });

            return buckets;
        }

        /// <summary>
        /// Create bucket with given name
        /// </summary>
        /// <param name="bucketKey">The bucket name.</param>
        public async Task CreateBucketAsync(string bucketKey)
        {
            await WithBucketApiAsync(async api =>
            {
                var payload = new PostBucketsPayload(bucketKey, /*allow*/null, PostBucketsPayload.PolicyKeyEnum.Persistent);
                await api.CreateBucketAsync(payload, /* use default (US region) */ null);
            });
        }

        public async Task DeleteBucketAsync(string bucketKey)
        {
            await WithBucketApiAsync(async api => await api.DeleteBucketAsync(bucketKey));
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

        public async Task UploadObjectAsync(string bucketKey, string objectName, Stream stream)
        {
            await WithObjectsApiAsync(async api => await api.UploadObjectAsync(bucketKey, objectName, 0, stream));
        }

        public async Task UploadChunkAsync(string bucketKey, string objectName, string contentRange, string sessionId, Stream stream)
        {
            await WithObjectsApiAsync(async api => await api.UploadChunkAsync(bucketKey, objectName, 0, contentRange, sessionId, stream));
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObjectAsync(string bucketKey, string oldName, string newName)
        {
            await WithObjectsApiAsync(async api =>
            {
                // OSS does not support renaming, so emulate it with more ineffective operations
                await api.CopyToAsync(bucketKey, oldName, newName);
                await api.DeleteObjectAsync(bucketKey, oldName);
            });
        }

        public async Task<Autodesk.Forge.Client.ApiResponse<dynamic>> GetObjectAsync(string bucketKey, string objectName)
        {
            return await WithObjectsApiAsync(async api => await api.GetObjectAsyncWithHttpInfo(bucketKey, objectName));
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public async Task CopyAsync(string bucketKey, string fromName, string toName)
        {
            await WithObjectsApiAsync(async api => await api.CopyToAsync(bucketKey, fromName, toName));
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public async Task DeleteAsync(string bucketKey, string objectName)
        {
            await WithObjectsApiAsync(async api => await api.DeleteObjectAsync(bucketKey, objectName));
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public async Task DownloadFileAsync(string bucketKey, string objectName, string localFullName)
        {
            await WithObjectsApiAsync(async api =>
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
        public async Task<dynamic> GetProfileAsync(string token)
        {
            var api = new UserProfileApi(new Configuration { AccessToken = token });
            return await api.GetUserProfileAsync();
        }

        /// <summary>
        /// Run action against Buckets OSS API.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private async Task WithBucketApiAsync(Func<BucketsApi, Task> action)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
                    {
                        var api = new BucketsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
                        await action(api);
                    });
        }

        /// <summary>
        /// Run action against Objects OSS API.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private async Task WithObjectsApiAsync(Func<ObjectsApi, Task> action)
        {
            await _refreshTokenPolicy.ExecuteAsync(async () =>
                    {
                        var api = new ObjectsApi { Configuration = { AccessToken = await TwoLeggedAccessToken } };
                        await action(api);
                    });
        }

        /// <summary>
        /// Run action against Objects OSS API.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private async Task<T> WithObjectsApiAsync<T>(Func<ObjectsApi, Task<T>> action)
        {
            return await _refreshTokenPolicy.ExecuteAsync(async () =>
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
