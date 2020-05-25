using System;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        private const string LocalCacheDir = "LocalCache";
        public const string VirtualCacheDir = "/data";

        public string BucketKey { get; }

        public Task<string> Nickname => _nickname.Value;
        private readonly Lazy<Task<string>> _nickname;

        /// <summary>
        /// Root dir for local cache.
        /// </summary>
        public string LocalRootName => _localRootDir.Value;
        private readonly Lazy<string> _localRootDir;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, string bucketKey = null)
        {
            var configuration = forgeConfigOptionsAccessor.Value.Validate();
            BucketKey = bucketKey ?? $"projects-{configuration.ClientId.Substring(0, 3)}-{configuration.HashString()}{Environment.GetEnvironmentVariable("BucketKeySuffix")}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));

            _localRootDir = new Lazy<string>(() =>
            {
                var localDir = Path.Combine(Directory.GetCurrentDirectory(), LocalCacheDir); // TODO: should the root dir be taken from config? or another class?

                // ensure the directory is exists
                Directory.CreateDirectory(localDir);

                return localDir;
            });
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
        /// Get project by its name.
        /// </summary>
        public Project GetProject(string projectName)
        {
            return new Project(projectName, LocalRootName);
        }

        /// <summary>
        /// Get project storage by project name.
        /// </summary>
        public ProjectStorage GetProjectStorage(string projectName)
        {
            var project = GetProject(projectName);
            return new ProjectStorage(project, this);
        }
    }
}
