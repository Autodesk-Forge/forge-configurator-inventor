using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace MigrationApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IForgeOSS _forgeOSS;
        private readonly ProjectWork _projectWork;
        private readonly UserResolver _userResolver;
        private readonly IBucketKeyProvider _bucketProvider;

        public  Worker(ILogger<Worker> logger, IForgeOSS forgeOSS, ProjectWork projectWork, UserResolver userResolver, IBucketKeyProvider bucketProvider)
        {
            _logger = logger;
            _forgeOSS = forgeOSS;
            _projectWork = projectWork;
            _userResolver = userResolver;
            _bucketProvider = bucketProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string bucketKey = "authd-lugxhey60gh0g6gytj3vqrhdmqnfv5kn-200-6521993cb7df5ed6108c37eb07d7b3ffe90d9082";
                await AdoptDefaultOnly(bucketKey);
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task AdoptDefaultOnly(string bucketKey)
        {
            _bucketProvider.SetBucketKey(bucketKey);
            OssBucket bucket = await _userResolver.GetBucketAsync();

            List<ObjectDetails> ossProjectFiles = await _forgeOSS.GetBucketObjectsAsync(bucket.BucketKey, "projects/");
            foreach (ObjectDetails file in ossProjectFiles)
            {
                string projectUrl = file.ObjectKey;
                string projectName = projectUrl.Split('/')[1];
                _logger.LogInformation("Project " + projectName + " is being adopted");
                WebApplication.State.Project project = await _userResolver.GetProjectAsync(projectName);

                await _forgeOSS.DownloadFileAsync(bucketKey, "attributes/" + projectName + "/metadata.json", "metadata.json");
                ProjectMetadata projectMetadata = Json.DeserializeFile<ProjectMetadata>("metadata.json");

                string signedUrl = await bucket.CreateSignedUrlAsync(project.OSSSourceModel, ObjectAccess.ReadWrite);

                ProjectInfo projectInfo = new ProjectInfo();
                projectInfo.Name = projectName;
                projectInfo.TopLevelAssembly = projectMetadata.TLA;

                try
                {
                    await _projectWork.AdoptAsync(projectInfo, signedUrl);
                    _logger.LogInformation("Project " + projectName + " was adopted");
                }
                catch(Exception e)
                {
                    _logger.LogError("Project " + projectName + " was not adopted\nException:" + e.Message);
                }
            }
        }

        /*
        public async Task<string> Refresh()
        {
            string returnValue = "";
            List<ObjectDetails> ossFiles = await _forgeOSS.GetBucketObjectsAsync(_bucket.BucketKey, "cache/");
            foreach (ObjectDetails file in ossFiles)
            {
                string[] fileParts = file.ObjectKey.Split('/');
                string project = fileParts[1];
                string hash = fileParts[2];
                string fileName = fileParts[3];
                if (fileName == "parameters.json")
                {
                    returnValue += "Project " + project + " (" + hash + ") is being updated\n";
                    string paramsFile = Path.Combine(_localCache.LocalRootName, "params.json");
                    await _bucket.DownloadFileAsync(file.ObjectKey, paramsFile);
                    InventorParameters inventorParameters = Json.DeserializeFile<InventorParameters>(paramsFile);
                    try
                    {
                        await _projectWork.DoSmartUpdateAsync(inventorParameters, project, true);
                        returnValue += "Project " + project + " (" + hash + ") was updated\n";
                    } catch(Exception e)
                    {
                        returnValue += "Project " + project + " (" + hash + ") update failed\nException: " + e.Message + "\n";
                    }
                }
            }

            return returnValue;
        }
        */
    }
}
