using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowParametersChangedController : ControllerBase
    {
        private readonly IForgeOSS _forgeOSS;
        private readonly ResourceProvider _resourceProvider;

        public ShowParametersChangedController(IForgeOSS forgeOSS, ResourceProvider resourceProvider)
        {
            _forgeOSS = forgeOSS;
            _resourceProvider = resourceProvider;
        }

        [HttpGet]
        public async Task<bool> Get()
        {
            bool result = true;

            ApiResponse<dynamic> ossObjectResponse = null;

            try
            {
                ossObjectResponse = await this._forgeOSS.GetObjectAsync(_resourceProvider.BucketKey, ONC.ShowPatametersChanged);
            } 
            catch(ApiException ex)
            {
               if (ex.ErrorCode != 404)
                {
                    throw;
                }
            }

            if(ossObjectResponse != null)
            {
                Stream objectStream = ossObjectResponse.Data;
                result = await JsonSerializer.DeserializeAsync<bool>(objectStream);
            }

            return result;
        }

        [HttpPost]
        public async Task<bool> Set([FromBody]bool show)
        {
            await this._forgeOSS.UploadObjectAsync(_resourceProvider.BucketKey, Json.ToStream(show), ONC.ShowPatametersChanged);
            return show;
        }
    }
}
