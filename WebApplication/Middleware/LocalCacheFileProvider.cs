using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using WebApplication.State;

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

        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = _fileProvider.GetFileInfo(subpath);
            if (! fileInfo.Exists && subpath.EndsWith("bubble.json"))
            {
                var pieces = subpath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                string projectName = pieces[1];
                string hash = pieces[2];

                var userResolver = _serviceProvider.GetService<UserResolver>();

                LoadSvfAsync(projectName, hash, userResolver).GetAwaiter().GetResult();

                //var userResolver = _serviceProvider.GetService<UserResolver>();

                //var projectStorage = await userResolver.GetProjectStorageAsync(projectName);
                //await projectStorage.EnsureSvfAsync(userResolver.GetBucketAsync(), hash);

                // and repeat
                fileInfo = _fileProvider.GetFileInfo(subpath);
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
