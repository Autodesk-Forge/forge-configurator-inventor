using System;
using System.IO;
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

            while (httpRequest.Cookies.TryGetValue("_t_", out var token))
            {
                // the expected path is like "/data/4EC4EC1C4C0082AB28582C8A50FFC2BF33E42356/Wrench/0B81352BCE7C9CEB8C8EAA7297A8AB64274C75A5/SVF/bubble.json"
                // 0 - 'root' for static files (data)
                // 1 - User dir (4EC4EC1C4C0082AB28582C8A50FFC2BF33E42356
                // 2 - Project ID (Wrench)
                // 3 - Parameters hash (0B81352BCE7C9CEB8C8EAA7297A8AB64274C75A5)
                // 4 - Subdir for SVF structure (SVF)
                // 5 - Manifest file for SVF (bubble.json)
                string[] pieces = httpRequest.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length != 6) break;

                string projectName = pieces[2];
                string hash = pieces[3];

                var userResolver = new UserResolver(resourceProvider, forgeOSS, forgeOptions, localCache)
                                    {
                                        Token = token
                                    };

                var projectStorage = await userResolver.GetProjectStorageAsync(projectName);

                // check if SVF dir already exists
                var svfDir = projectStorage.GetLocalNames(hash).SvfDir;
                if (Directory.Exists(svfDir)) break;
                
                // download and extract SVF
                var bucket = await userResolver.GetBucketAsync();
                await projectStorage.EnsureSvfAsync(bucket, hash);

                break;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
