using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WebApplication.Processing;
using WebApplication.Utilities;
using Xunit;

namespace WebApplication.Tests
{
    public class PublisherFixture : IDisposable
    {
        public Forge Forge { get; private set; }
        public string BucketName { get; private set; }
        public ResourceProvider ResourceProvider { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public DirectoryInfo TestFileDirectory { get; private set; }

        public PublisherFixture()
        {
            Forge = new Forge();
            IOptions<ForgeConfiguration> resourceProviderOptions = Options.Create(Forge.Configuration);
            ResourceProvider = new ResourceProvider(resourceProviderOptions, Forge.FdaClient);
            BucketName = Guid.NewGuid().ToString();
            Forge.OSSClient.CreateBucketAsync(BucketName).Wait();
            HttpClient = new HttpClient();
            TestFileDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
        }

        public void Dispose()
        {
            Forge.OSSClient.DeleteBucket(BucketName);
            TestFileDirectory.Delete(true);
            HttpClient.Dispose();
        }
    }

    enum WorkItemTestType
    {
        IPT,
        IAM
    }

    public class PublisherTest : IClassFixture<PublisherFixture>
    {
        readonly PublisherFixture fixture;
        string testIptUrl = "http://testipt.s3-us-west-2.amazonaws.com/PictureInFormTest.ipt";
        string testZippedIamUrl = "http://testipt.s3-us-west-2.amazonaws.com/Basic.zip";
        string testIamPathInZip = "iLogicBasic1.iam";

        public PublisherTest(PublisherFixture fixture)
        {
            this.fixture = fixture;
        }

        private async Task<string> DownloadTestComparisonFile(string url, string name)
        {
            HttpResponseMessage response = await fixture.HttpClient.GetAsync(url);
            string path = Path.Combine(fixture.TestFileDirectory.FullName, name);
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
            byte[] generatedOutputFileBytes = await fixture.HttpClient.GetByteArrayAsync(outputFileUrl);

            // first confirm they are the same number of bytes
            Assert.Equal(expectedBytes.Length, generatedOutputFileBytes.Length);

            // then confirm each byte is the same
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                Assert.True(expectedBytes[i] == generatedOutputFileBytes[i], $"unequal bytes at index {i}");
            }
        }

        private async Task WorkItemTest(WorkItemTestType testType, string outputFileName, ForgeAppConfigBase definition, string expectedResultFileName)
        {
            string outputUrl = await fixture.Forge.OSSClient.CreateSignedResourceAsync(fixture.BucketName, outputFileName);

            // Run work item and wait for it to finish
            WorkItemStatus status = null;
            if (testType == WorkItemTestType.IPT)
            {
                status = await definition.ProcessIpt(testIptUrl, outputUrl);
            }
            else if (testType == WorkItemTestType.IAM)
            {
                status = await definition.ProcessZippedIam(testZippedIamUrl, testIamPathInZip, outputUrl);
            }

            // get report
            string report = await fixture.HttpClient.GetStringAsync(status.ReportUrl);

            // check if the job was successful
            Assert.True(status.Status == Status.Success, report);

            if (!expectedResultFileName.EndsWith(".zip")) // TODO handle SVF zip files that have different sizes each time
            {
                await CompareOutputFileBytes(expectedResultFileName, outputUrl);
            }
        }

        [Fact]
        public async Task CreateThumbnailIptTestAsync()
        {
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new CreateThumbnailDefinition(publisher);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/pictureInFormIptThumbnail.png", "pictureInFormIptThumbnail.png");
            await WorkItemTest(WorkItemTestType.IPT, "Thumbnail.png", definition, testComparisonFilePath);
        }

        [Fact]
        public async Task CreateThumbnailZippedIamTestAsync()
        {
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new CreateThumbnailDefinition(publisher);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamThumbnail.png", "iLogicBasic1IamThumbnail.png");
            await WorkItemTest(WorkItemTestType.IAM, "Thumbnail.png", definition, testComparisonFilePath);
        }

        [Fact]
        public async Task ExtractParametersIptTestAsync()
        {
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new ExtractParametersDefinition(publisher);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/pictureInFormIptDocumentParams.json", "pictureInFormIptDocumentParams.json");
            await WorkItemTest(WorkItemTestType.IPT, "documentParams.json", definition, testComparisonFilePath);
        }

        [Fact]
        public async Task ExtractParametersZippedIamTestAsync()
        {
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new ExtractParametersDefinition(publisher);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamDocumentParams.json", "iLogicBasic1IamDocumentParams.json");
            await WorkItemTest(WorkItemTestType.IAM, "documentParams.json", definition, testComparisonFilePath);
        }

        [Fact]
        public async Task CreateSvfIptTestAsync()
        {
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new CreateSvfDefinition(publisher);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/pictureInFormIptSvf.zip", "pictureInFormIptSvf.zip");
            await WorkItemTest(WorkItemTestType.IPT, "pictureInFormIptSvf.zip", definition, testComparisonFilePath);
        }

        [Fact]
        public async Task CreateSvfZippedIamTestAsync()
        {
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new CreateSvfDefinition(publisher);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamSvf.zip", "iLogicBasic1IamSvf.zip");
            await WorkItemTest(WorkItemTestType.IAM, "iLogicBasic1IamSvf.zip", definition, testComparisonFilePath);
        }
    }
}
