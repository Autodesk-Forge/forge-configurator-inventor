using System;
using System.Net.Http;
using System.Text.RegularExpressions;
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

        public PublisherFixture()
        {
            Forge = new Forge();
            IOptions<ForgeConfiguration> resourceProviderOptions = Options.Create(Forge.Configuration);
            ResourceProvider = new ResourceProvider(resourceProviderOptions, Forge.FdaClient);
            BucketName = Guid.NewGuid().ToString();
            Forge.OSSClient.CreateBucketAsync(BucketName).Wait();
        }

        public void Dispose()
        {
            Forge.OSSClient.DeleteBucket(BucketName);
        }
    }

    public class PublisherTest : IClassFixture<PublisherFixture>
    {
        PublisherFixture fixture;

        public PublisherTest(PublisherFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task CreateThumbnailTestAsync()
        {
            string inventorDocUrl = "http://testipt.s3-us-west-2.amazonaws.com/PictureInFormTest.ipt";
            string outputThumbnailUrl = await fixture.Forge.OSSClient.CreateSignedResourceAsync(fixture.BucketName, "Thumbnail.png");
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new CreateThumbnailDefinition(publisher);

            // Run work item and wait for it to finish
            WorkItemStatus status = await definition.ProcessDocument(inventorDocUrl, outputThumbnailUrl);

            // get report
            var httpclient = new HttpClient();
            string report = await httpclient.GetStringAsync(status.ReportUrl);

            // check if the job was successful
            Assert.True(status.Status == Status.Success, report);

            // TODO download result, compare it to expected value
            
        }

        [Fact]
        public async Task ExtractParametersTestAsync()
        {
            string inventorDocUrl = "http://testipt.s3-us-west-2.amazonaws.com/PictureInFormTest.ipt";
            string outputJsonUrl = await fixture.Forge.OSSClient.CreateSignedResourceAsync(fixture.BucketName, "documentParams.json");
            var publisher = new Publisher(fixture.Forge.FdaClient, new NullLogger<Publisher>(), fixture.ResourceProvider);
            var definition = new ExtractParametersDefinition(publisher);

            // Run work item and wait for it to finish
            WorkItemStatus status = await definition.ProcessDocument(inventorDocUrl, outputJsonUrl);

            // get report
            var httpclient = new HttpClient();
            string report = await httpclient.GetStringAsync(status.ReportUrl);

            // check if the job was successful
            Assert.True(status.Status == Status.Success, report);

            // download result, compare it to expected value
            string json = await httpclient.GetStringAsync(outputJsonUrl);
            json = Regex.Replace(json, @"\s", string.Empty);
            string expectedValue = "{\"SquareOrRound\":{\"values\":[\"\\\"Round\\\"\",\"\\\"Square\\\"\"],\"value\":\"\\\"Square\\\"\",\"unit\":\"Text\"},\"IsSquare\":{\"value\":\"True\",\"unit\":\"Boolean\"},\"IsRound\":{\"value\":\"False\",\"unit\":\"Boolean\"}}";

            Assert.True(json == expectedValue, "Wrong content in documentParams.json");
        }
    }
}
