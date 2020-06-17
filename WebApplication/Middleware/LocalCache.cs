using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace WebApplication.Middleware
{
    public class LocalCache
    {
        private const string LocalCacheDir = "LocalCache";
        public const string VirtualCacheDir = "/data";

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir);

        /// <summary>
        /// Get URL pointing for the data file.
        /// </summary>
        /// <param name="localFileName">Full filename. Must be under "local cache root"</param>
        public string ToDataUrl(string localFileName)
        {
            if (!localFileName.StartsWith(LocalRootName, StringComparison.InvariantCultureIgnoreCase))
                throw new ApplicationException("Attempt to generate URL for non-data file");

            string relativeName = localFileName.Substring(LocalRootName.Length);
            return VirtualCacheDir + relativeName.Replace('\\', '/');
        }

        /// <summary>
        /// expose local cache dir as 'data' virtual dir to serve locally cached OSS files
        /// </summary>
        public void Serve(IApplicationBuilder app)
        {
            Directory.CreateDirectory(LocalRootName);
 
            app.UseStaticFiles(new StaticFileOptions
            {
                // make sure that directory exists
                FileProvider = new PhysicalFileProvider(LocalRootName),
                RequestPath = new PathString(VirtualCacheDir),
                ServeUnknownFileTypes = true
            });
        }
    }
}
