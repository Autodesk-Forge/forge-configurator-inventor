using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Middleware
{
    public class LocalCacheFileProvider : IFileProvider, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PhysicalFileProvider _fileProvider;

        public LocalCacheFileProvider(string root, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _fileProvider = new PhysicalFileProvider(root);
        }

        public void Dispose()
        {
            _fileProvider.Dispose();
        }

        public IFileInfo GetFileInfo(string subPath)
        {
            var fileInfo = _fileProvider.GetFileInfo(subPath);
            if (! fileInfo.Exists && subPath.EndsWith(".bubble.json"))
            {
                var pieces = subPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                string projectName = pieces[1];
                string hash = pieces[2];

                var fileName = pieces[^1];
                string token = fileName.Substring(0, fileName.Length - ".bubble.json".Length);
                // TODO: handle "no token" situation

                UserResolver userResolver = new UserResolver(_serviceProvider.GetService<ResourceProvider>(), _serviceProvider.GetService<IForgeOSS>(), 
                    _serviceProvider.GetService<IOptions<ForgeConfiguration>>(), _serviceProvider.GetService<LocalCache>())
                {
                    Token = token
                };

                LoadSvfAsync(projectName, hash, userResolver).GetAwaiter().GetResult();

                // and repeat
                string realSubPath = Path.Combine(subPath.Substring(0, subPath.Length - fileName.Length), "bubble.json");
                fileInfo = _fileProvider.GetFileInfo(realSubPath);
            }

            return fileInfo;
        }

        private async Task LoadSvfAsync(string projectName, string hash, UserResolver userResolver)
        {
            var projectStorage = await userResolver.GetProjectStorageAsync(projectName);
            var bucket = await userResolver.GetBucketAsync();

            await projectStorage.EnsureSvfAsync(bucket, hash);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _fileProvider.GetDirectoryContents(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _fileProvider.Watch(filter);
        }
    }
}
