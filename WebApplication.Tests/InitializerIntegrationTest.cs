using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.Processing;
using WebApplication.Utilities;
using Xunit;

namespace WebApplication.Tests
{
    public class InitializerIntegrationTest : IDisposable
    {
        const string testZippedIamUrl = "http://testipt.s3-us-west-2.amazonaws.com/Basic.zip";
        const string testIamPathInZip = "iLogicBasic1.iam";

        ForgeOSS forgeOSS;
        string projectsBucketKey;
        Initializer initializer;
        DirectoryInfo testFileDirectory;
        HttpClient httpClient;

        public InitializerIntegrationTest()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddForgeAlternativeEnvironmentVariables()
                .Build();

            ForgeConfiguration forgeConfiguration = configuration.GetSection("Forge").Get<ForgeConfiguration>();
            IOptions<ForgeConfiguration> forgeConfigOptions = Options.Create(forgeConfiguration);

            forgeOSS = new ForgeOSS(forgeConfigOptions, new NullLogger<ForgeOSS>());

            var httpMessageHandler = new ForgeHandler(Options.Create(forgeConfiguration))
            {
                InnerHandler = new HttpClientHandler()
            };
            var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
            DesignAutomationClient designAutomationClient = new DesignAutomationClient(forgeService);

            projectsBucketKey = Guid.NewGuid().ToString();
            ProjectsBucket projectsBucket = new ProjectsBucket { Name = projectsBucketKey };
            IOptions<ProjectsBucket> projectsBucketOptions = Options.Create(projectsBucket);

            var resourceProvider = new ResourceProvider(projectsBucketOptions, forgeConfigOptions, designAutomationClient);
            var publisher = new Publisher(designAutomationClient, new NullLogger<Publisher>(), resourceProvider);

            AppBundleZipPaths appBundleZipPathsConfiguration = new AppBundleZipPaths()
            {
                CreateSVF = "..\\..\\..\\..\\AppBundles\\Output\\CreateSVFPlugin.bundle.zip",
                CreateThumbnail = "..\\..\\..\\..\\AppBundles\\Output\\CreateThumbnailPlugin.bundle.zip",
                ExtractParameters = "..\\..\\..\\..\\AppBundles\\Output\\ExtractParametersPlugin.bundle.zip"
            };
            IOptions<AppBundleZipPaths> appBundleZipPathsOptions = Options.Create(appBundleZipPathsConfiguration);

            var fdaClient = new FdaClient(publisher, appBundleZipPathsOptions);
            DefaultProjectsConfiguration defaultProjectsConfiguration = new DefaultProjectsConfiguration
            {
                Projects = new DefaultProjectConfiguration[] { new DefaultProjectConfiguration { Url = testZippedIamUrl, TopLevelAssembly = testIamPathInZip } }
            };
            IOptions<DefaultProjectsConfiguration> defaultProjectsOptions = Options.Create(defaultProjectsConfiguration);
            initializer = new Initializer(forgeOSS, resourceProvider, new NullLogger<Initializer>(), fdaClient, defaultProjectsOptions);

            testFileDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            httpClient = new HttpClient();
        }

        public void Dispose()
        {
            testFileDirectory.Delete(true);
            httpClient.Dispose();
        }

        private async Task<string> DownloadTestComparisonFile(string url, string name)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            string path = Path.Combine(testFileDirectory.FullName, name);
            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }
            return path;
        }

        private async Task CompareOutputFileBytes(string expectedResultFileName, string outputFileUrl)
        {
            // download result, compare it to expected value
            byte[] expectedBytes = File.ReadAllBytes(expectedResultFileName);
            byte[] generatedOutputFileBytes = await httpClient.GetByteArrayAsync(outputFileUrl);

            // first confirm they are the same number of bytes
            Assert.Equal(expectedBytes.Length, generatedOutputFileBytes.Length);

            // then confirm each byte is the same
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                Assert.True(expectedBytes[i] == generatedOutputFileBytes[i], $"unequal bytes at index {i}");
            }
        }

        [Fact]
        public async Task InitializeTestAsync()
        {
            await initializer.Clear();
            await initializer.Initialize();

            // check thumbnail generated
            List<ObjectDetails> objects = await forgeOSS.GetBucketObjects(projectsBucketKey, "attributes-Basic.zip-thumbnail.png");
            Assert.Single(objects);
            string signedOssUrl = await forgeOSS.CreateSignedUrl(projectsBucketKey, objects[0].ObjectKey);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamThumbnail.png", "iLogicBasic1IamThumbnail.png");
            await CompareOutputFileBytes(testComparisonFilePath, signedOssUrl);

            // check parameters generated with hashed name
            objects = await forgeOSS.GetBucketObjects(projectsBucketKey, "cache-Basic.zip-DE160BCE36BA38F7D3778C588F3C4D69C50902D3-parameters.json");
            Assert.Single(objects);
            signedOssUrl = await forgeOSS.CreateSignedUrl(projectsBucketKey, objects[0].ObjectKey);
            testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamDocumentParams.json", "iLogicBasic1IamDocumentParams.json");
            await CompareOutputFileBytes(testComparisonFilePath, signedOssUrl);

            // check model view generated with hashed name (zip of SVF size/content varies slightly each time so we can only check if it was created)
            objects = await forgeOSS.GetBucketObjects(projectsBucketKey, "cache-Basic.zip-DE160BCE36BA38F7D3778C588F3C4D69C50902D3-model-view.zip");
            Assert.Single(objects);

            await initializer.Clear();
        }
    }
}
