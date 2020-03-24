using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;

namespace IoConfigDemo
{
    class Forge : IForge
    {
        private static readonly Scope[] _scope = { Scope.DataRead, Autodesk.Forge.Scope.BucketCreate, Autodesk.Forge.Scope.BucketRead };

        // Initialize the 2-legged oAuth 2.0 client.
        private static readonly TwoLeggedApi _twoLeggedApi = new TwoLeggedApi();

        private string _twoLeggedAccessToken;
        private async Task<string> GetTwoLeggedAccessToken()
        {
            if (_twoLeggedAccessToken == null)
            {
                dynamic bearer = await _2leggedAsync();
                _twoLeggedAccessToken = bearer.access_token;
            }

            return _twoLeggedAccessToken;
        }

        private readonly ForgeConfiguration _options;

        public Forge(IOptionsMonitor<ForgeConfiguration> optionsAccessor)
        {
            ForgeConfiguration options = optionsAccessor.CurrentValue;
            if (string.IsNullOrEmpty(options.ClientId)) throw new ArgumentException("Forge Client ID is not provided.");
            if (string.IsNullOrEmpty(options.ClientSecret)) throw new ArgumentException("Forge Client Secret is not provided.");

            _options = optionsAccessor.CurrentValue;
        }

        private async Task<dynamic> _2leggedAsync()
        {
            // Call the asynchronous version of the 2-legged client with HTTP information
            // HTTP information helps to verify if the call was successful as well as read the HTTP transaction headers.
            Autodesk.Forge.Client.ApiResponse<dynamic> response = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(_options.ClientId, _options.ClientSecret, oAuthConstants.CLIENT_CREDENTIALS, _scope);

            if (response.StatusCode != StatusCodes.Status200OK)
            {
                throw new Exception("Request failed! (with HTTP response " + response.StatusCode + ")");
            }

            // The JSON response from the oAuth server is the Data variable and has already been parsed into a DynamicDictionary object.
            return response.Data;
        }

        public async Task<List<ObjectDetails>> GetBucketObjects(string bucketKey)
        {
            await EnsureBucket(bucketKey);

            ObjectsApi objectsApi = new ObjectsApi();
            objectsApi.Configuration.AccessToken = await GetTwoLeggedAccessToken();
            var objects = new List<ObjectDetails>();

            dynamic objectsList = await objectsApi.GetObjectsAsync(bucketKey);
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
        /// Make sure the bucket is exists.  Create if necessary.
        /// </summary>
        /// <param name="bucketName">The bucket name.</param>
        private async Task EnsureBucket(string bucketName)
        {
            var api = new BucketsApi { Configuration = { AccessToken = await GetTwoLeggedAccessToken() }};

            try
            {
                // the API call will throw an exception if the bucket is missing,
                // so we don't care about the response, just need to ensure about succeeded call
                await api.GetBucketDetailsAsync(bucketName);
            }
            catch (ApiException e) when(e.ErrorCode == StatusCodes.Status404NotFound) // try to create the bucket if error is "Not Found"
            {
                var payload = new PostBucketsPayload(bucketName, /*allow*/null, PostBucketsPayload.PolicyKeyEnum.Persistent);
                await api.CreateBucketAsync(payload);
            }
        }
    }
}