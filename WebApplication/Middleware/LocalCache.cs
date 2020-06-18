using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace WebApplication.Middleware
{
    /// <summary>
    /// For performance reason - some important generated files are cached locally,
    /// this class encapsulate related logic and allows access to it as to static files.
    /// </summary>
    public class LocalCache
    {
        private const string LocalCacheDir = "LocalCache";
        public const string VirtualCacheDir = "/data";

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir);

        public LocalCache()
        {
            Directory.CreateDirectory(LocalRootName);
        }

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
        /// Expose local cache dir as 'data' virtual dir to serve locally cached OSS files
        /// </summary>
        public void Serve(IApplicationBuilder app)
        {
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
