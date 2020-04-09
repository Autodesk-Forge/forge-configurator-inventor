using System;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IoConfigDemo
{
    public class Initializer
    {
        private readonly IForge _forge;
        private readonly BucketNameProvider _bucketNameProvider;
        private readonly ILogger<Initializer> _logger;
        public Initializer(IForge forge, BucketNameProvider bucketNameProvider, ILogger<Initializer> logger)
        {
            _forge = forge;
            _bucketNameProvider = bucketNameProvider;
            _logger = logger;
        }

        public async Task Initialize()
        {
            _logger.LogInformation($"Initializing base data");
            await _forge.CreateBucket(_bucketNameProvider.BucketName);
            _logger.LogInformation($"Bucket {_bucketNameProvider.BucketName} created");
            
            await Task.WhenAll(
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, new ObjectNameProvider("Project1.zip").SourceModel),
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, new ObjectNameProvider("Project2.zip").SourceModel),
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, new ObjectNameProvider("Project3.ipt").SourceModel)
            );
            _logger.LogInformation($"Added empty projects.");
        }

        public async Task Clear()
        {
            try {
                await _forge.DeleteBucket(_bucketNameProvider.BucketName);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            } catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound) {
                _logger.LogInformation($"Nothing to delete because bucket {_bucketNameProvider.BucketName} does not exists yet");
            }
            
        }
    }
}