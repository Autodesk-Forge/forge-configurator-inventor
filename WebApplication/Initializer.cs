using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication
{
    public class Initializer
    {
        private readonly IForgeOSS _forge;
        private readonly ResourceProvider _resourceProvider;
        private readonly ILogger<Initializer> _logger;
        private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
        private readonly FdaClient _fdaClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Arranger _arranger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Initializer(IForgeOSS forge, ResourceProvider resourceProvider, ILogger<Initializer> logger,
                            FdaClient fdaClient, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                            IHttpClientFactory httpClientFactory, Arranger arranger)
        {
            _forge = forge;
            _resourceProvider = resourceProvider;
            _logger = logger;
            _fdaClient = fdaClient;
            _httpClientFactory = httpClientFactory;
            _arranger = arranger;
            _defaultProjectsConfiguration = optionsAccessor.Value;
        }

        public async Task InitializeAsync()
        {
            using var scope = _logger.BeginScope("Init");

            // create bundles and activities
            await _fdaClient.InitializeAsync();

            _logger.LogInformation("Initializing base data");

            await _forge.CreateBucketAsync(_resourceProvider.BucketKey);
            _logger.LogInformation($"Bucket {_resourceProvider.BucketKey} created");

            // download default project files from the public location
            // specified by the appsettings.json
            var httpClient = _httpClientFactory.CreateClient();

            foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
            {
                var projectUrl = defaultProjectConfig.Url;
                var tlaFilename = defaultProjectConfig.TopLevelAssembly;

                string[] urlParts = projectUrl.Split("/");
                string projectName = urlParts[^1];
                var project = new Project(projectName);

                _logger.LogInformation($"Download {projectUrl}");
                using (HttpResponseMessage response = await httpClient.GetAsync(projectUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    _logger.LogInformation("Upload to the app bucket");

                    // store project locally
                    string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                    using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    {
                        await response.Content.CopyToAsync(fs);

                        // determine if we need to upload in chunks or in one piece
                        long sizeToUpload = fs.Length;
                        long chunkMBSize = 5;
                        long chunkSize = chunkMBSize * 1024 * 1024; // 2MB is minimal

                        // use chunks for all files greater than chunk size
                        if (sizeToUpload > chunkSize)
                        {
                            long chunksCnt = (long)((sizeToUpload + chunkSize - 1) / chunkSize);

                            _logger.LogInformation($"Uploading in {chunksCnt} x {chunkMBSize}MB chunks");

                            string sessionId = Guid.NewGuid().ToString();
                            long begin = 0;
                            long end = chunkSize - 1;
                            long count = chunkSize;
                            byte[] buffer = new byte[count];

                            for (int idx = 0; idx < chunksCnt; idx++)
                            {
                                // jump to requested position
                                fs.Seek(begin, SeekOrigin.Begin);
                                fs.Read(buffer, 0, (int)count);
                                using (MemoryStream chunkStream = new MemoryStream(buffer, 0, (int)count))
                                {
                                    string contentRange = string.Format($"bytes {begin}-{end}/{sizeToUpload}");
                                    await _forge.UploadChunkAsync(_resourceProvider.BucketKey, chunkStream, project.OSSSourceModel, contentRange, sessionId);
                                }
                                begin = end + 1;
                                chunkSize = ((begin + chunkSize > sizeToUpload) ? sizeToUpload - begin : chunkSize);
                                // for the last chunk there should be smaller count of bytes to read
                                if (chunkSize > 0 && chunkSize != count)
                                {
                                    // reset to the new size for the LAST chunk
                                    count = chunkSize;
                                }

                                end = begin + chunkSize - 1;
                            }
                        }
                        else
                        {
                            // jump to beginning
                            fs.Seek(0, SeekOrigin.Begin);
                            await _forge.UploadObjectAsync(_resourceProvider.BucketKey, fs, project.OSSSourceModel);
                        }
                    }
                    // delete local temporary file
                    File.Delete(path);
                }

                _logger.LogInformation("Adopt the project");
                await AdoptAsync(project, tlaFilename);
            }

            _logger.LogInformation("Added default projects.");
        }

        public async Task ClearAsync()
        {
            try
            {
                await _forge.DeleteBucketAsync(_resourceProvider.BucketKey);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                _logger.LogInformation($"Nothing to delete because bucket {_resourceProvider.BucketKey} does not exists yet");
            }

            // delete bundles and activities
            await _fdaClient.CleanUpAsync();
        }

        /// <summary>
        /// Adapt the project.
        /// </summary>
        private async Task AdoptAsync(Project project, string tlaFilename)
        {
            var inputDocUrl = await _forge.CreateSignedUrlAsync(_resourceProvider.BucketKey, project.OSSSourceModel);

            var adoptionData = await _arranger.ForAdoptionAsync(inputDocUrl, tlaFilename);

            var status = await _fdaClient.AdoptAsync(adoptionData); // ER: think: it's a business logic, so it might not deal with low-level WI and status
            if (status.Status != Status.Success)
            {
                _logger.LogError($"Failed to adopt {project.Name}");
            }
            else
            {
                // rearrange generated data according to the parameters hash
                await _arranger.DoAsync(project);
            }
        }
    }
}
