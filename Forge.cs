using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;

namespace IoConfigDemo {
	class Forge {
		// Initialize the oAuth 2.0 client configuration fron enviroment variables
		private static string FORGE_CLIENT_ID = Environment.GetEnvironmentVariable ("FORGE_CLIENT_ID");
		private static string FORGE_CLIENT_SECRET = Environment.GetEnvironmentVariable ("FORGE_CLIENT_SECRET");
		private static Scope[] _scope = new Scope[] { Scope.DataRead };

		// Intialize the 2-legged oAuth 2.0 client.
		private static TwoLeggedApi _twoLeggedApi =new TwoLeggedApi() ;

        private string _twoLeggedAccessToken;
        private async Task<string> GetTwoLeggedAccessToken()
        {
            if (_twoLeggedAccessToken == null)
            {
                dynamic bearer = await _2leggedAsync();

                if (bearer == null) {
                    Debug.Print("Failed to get 2 legged access token");
                }

                _twoLeggedAccessToken = bearer.access_token;
            }

            return _twoLeggedAccessToken;
        }

		private static async Task<dynamic> _2leggedAsync () {
			try {
				// Call the asynchronous version of the 2-legged client with HTTP information
				// HTTP information will help you to verify if the call was successful as well
				// as read the HTTP transaction headers.
				ApiResponse<dynamic> bearer = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, oAuthConstants.CLIENT_CREDENTIALS, _scope);
				
                if (bearer.StatusCode != 200)
                {
					throw new Exception ("Request failed! (with HTTP response " + bearer.StatusCode + ")") ;
                }

				// The JSON response from the oAuth server is the Data variable and has been
				// already parsed into a DynamicDictionary object.
				return bearer.Data;
			} catch {
				return null;
			}
		}

        public async Task<List<ObjectDetails>> GetBucketObjects(string bucketKey)
        {
            ObjectsApi objectsApi = new ObjectsApi();
            objectsApi.Configuration.AccessToken = await GetTwoLeggedAccessToken();
            List<ObjectDetails> objects = new List<ObjectDetails>();
            
            try
            {
                dynamic objectsList = await objectsApi.GetObjectsAsync(bucketKey);
                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
                {
                    ObjectDetails details = new ObjectDetails {
                        BucketKey = objInfo.Value.bucketKey,
                        ObjectId = objInfo.Value.objectId,
                        ObjectKey = objInfo.Value.objectKey,
                        Sha1 = System.Text.Encoding.ASCII.GetBytes(objInfo.Value.sha1),
                        Size = (int?)objInfo.Value.size,
                        Location = objInfo.Value.location
                    };
                    objects.Add(details);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Exception when calling ObjectsApi.GetObjects: " + ex.Message);
            }

            return objects;
        }
	}
}