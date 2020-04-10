using System;
using System.Threading.Tasks;
using WebApplication.Processing;
using Xunit;

namespace WebApplication.Tests
{
    public class PublisherFixture: IDisposable
    {
        public Forge Forge { get; private set; }
        public string BucketName { get; private set; }

        public PublisherFixture()
        {
            Forge = new Forge();
            BucketName = Guid.NewGuid().ToString();
            Forge.OSSClient.CreateBucketAsync(BucketName).Wait();
        }

        public void Dispose()
        {
            Forge.OSSClient.DeleteBucket(BucketName);
        }
    }

    public class PublisherTest: IClassFixture<PublisherFixture>
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

            var definition = new CreateThumbnailDefinition(inventorDocUrl, outputThumbnailUrl);
            var publisher = new Publisher(definition, fixture.Forge.FdaClient);
            await publisher.RunWorkItemAsync();

            // TODO wait for job to finish, download result, compare it to expected value
        }
    }
}
