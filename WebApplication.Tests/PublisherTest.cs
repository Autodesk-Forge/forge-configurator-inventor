using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Extensions.Logging;
using Moq;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Utilities;
using Xunit;

namespace WebApplication.Tests
{
    public class PublisherTest
    {
        private class CallbackDataProvider : IEnumerable<object[]>
        {
            private readonly Publisher _publisher;

            private readonly Dictionary<string, IArgument> _workItemArgs;
            private readonly Mock<IForgeAppBase> _configMock;
            private readonly Mock<IWorkItemsApi> _workItemsApiMock;

            private readonly string _trackingKey = Guid.NewGuid().ToString("N");

            private readonly WorkItemStatus _workItemStatus = new WorkItemStatus
            {
                Status = Status.FailedLimitDataSize
            };

            public CallbackDataProvider()
            {
                var logger = new SerilogLoggerFactory().CreateLogger<Publisher>();
                var resourceProviderMock = new Mock<IResourceProvider>();
                var postProcessingMock = new Mock<IPostProcessing>();
                var publisherConfiguration = new PublisherConfiguration
                {
                    CallbackUrlBase = "http://fci/complete/",
                    CompletionCheck = CompletionCheck.Callback
                };
                var publisherConfigurationOptions = Options.Create(publisherConfiguration);

                var workItem = new WorkItem
                {
                    ActivityId = "nickname.activityId+activityLabel",
                    Arguments = _workItemArgs
                };

                _workItemsApiMock = new Mock<IWorkItemsApi>();

                _workItemsApiMock.Setup(mock => mock.CreateWorkItemAsync(
                        It.Is<WorkItem>(wi => wi.ActivityId.Equals(workItem.ActivityId)
                                              && ((XrefTreeArgument) wi.Arguments["onComplete"]).Url.StartsWith(
                                                  publisherConfiguration.CallbackUrlBase)),
                        null, null, true))
                    .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                    {
                        Status = Status.Success
                    })));

                var guidGenerator = new Mock<IGuidGenerator>();
                guidGenerator.Setup(mock => mock.GenerateGuid()).Returns(_trackingKey);

                _publisher = new Publisher(
                    null,
                    logger,
                    resourceProviderMock.Object,
                    postProcessingMock.Object,
                    publisherConfigurationOptions,
                    _workItemsApiMock.Object,
                    guidGenerator.Object);

                resourceProviderMock.SetupGet(mock => mock.Nickname).Returns(Task.FromResult("nickname"));

                _configMock = new Mock<IForgeAppBase>();
                _configMock.Setup(mock => mock.ActivityId).Returns("activityId");
                _configMock.Setup(mock => mock.ActivityLabel).Returns("activityLabel");

                _workItemArgs = new Dictionary<string, IArgument>();
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    _publisher, 
                    _workItemArgs, 
                    _configMock.Object, 
                    _trackingKey, 
                    _workItemStatus, 
                    _workItemsApiMock
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(CallbackDataProvider))]
        public void RunWorkItemAsyncUsingCallback(Publisher publisher, Dictionary<string, IArgument> workItemArgs, 
            IForgeAppBase config, string trackingKey, WorkItemStatus workItemStatus, 
            Mock<IWorkItemsApi> workItemsApiMock)
        {
            var workItemTask = publisher.RunWorkItemAsync(workItemArgs, config);
            publisher.NotifyTaskIsCompleted(trackingKey, workItemStatus);
            workItemTask.Wait();
            
            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(workItemStatus.Status, workItemTask.Result.Status);
        }
    }
}