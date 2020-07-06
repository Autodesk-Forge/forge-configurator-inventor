using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Mvc;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowParametersChangedController : ControllerBase
    {
        private readonly UserResolver _userResolver;

        public ShowParametersChangedController(UserResolver userResolver)
        {
            _userResolver = userResolver;
        }

        [HttpGet]
        public async Task<bool> Get()
        {
            bool result = true;

            ApiResponse<dynamic> ossObjectResponse = null;

            try
            {
                var bucket = await _userResolver.GetBucketAsync();
                ossObjectResponse = await bucket.GetObjectAsync(ONC.ShowParametersChanged);
            } 
            catch (ApiException ex) when (ex.ErrorCode == 404)
            {
                // the file is not found. Just swallow the exception
            }

            if(ossObjectResponse != null)
            {
                using (Stream objectStream = ossObjectResponse.Data)
                {
                    result = await JsonSerializer.DeserializeAsync<bool>(objectStream);
                }
            }

            return result;
        }

        [HttpPost]
        public async Task<bool> Set([FromBody]bool show)
        {
            var bucket = await _userResolver.GetBucketAsync();
            await bucket.UploadObjectAsync(ONC.ShowParametersChanged, Json.ToStream(show));
            return show;
        }
    }
}
