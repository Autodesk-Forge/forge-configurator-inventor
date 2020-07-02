using System;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Middleware
{
    public class SvfRestore
    {
        private readonly RequestDelegate _next;

        public SvfRestore(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ResourceProvider resourceProvider, IForgeOSS forgeOSS,
                                        IOptions<ForgeConfiguration> forgeOptions, LocalCache localCache)
        {
            var httpRequest = context.Request;

            if (httpRequest.Cookies.TryGetValue("_t_", out var token))
            {
                var pieces = httpRequest.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
                string projectName = pieces[2];
                string hash = pieces[3];

                UserResolver userResolver = new UserResolver(resourceProvider, forgeOSS, forgeOptions, localCache)
                {
                    Token = token
                };

                var projectStorage = await userResolver.GetProjectStorageAsync(projectName);
                var bucket = await userResolver.GetBucketAsync();

                await projectStorage.EnsureSvfAsync(bucket, hash);
            }



            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
