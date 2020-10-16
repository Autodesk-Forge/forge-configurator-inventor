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
        private readonly Dictionary<string, IArgument> _workItemArgs;
        private readonly Mock<IForgeAppBase> _configMock;

        public PublisherTest()
        {
            _workItemArgs = new Dictionary<string, IArgument>();

            _configMock = new Mock<IForgeAppBase>();
            _configMock.Setup(mock => mock.ActivityId).Returns("activityId");
            _configMock.Setup(mock => mock.ActivityLabel).Returns("activityLabel");
        }

        private class CallbackDataProvider : IEnumerable<object[]>
        {
            private readonly Publisher _publisher;

            private readonly Mock<IWorkItemsApi> _workItemsApiMock;

            private const string _trackingKey = "cd26ccf675d64521884f1693c62ed303";

            public CallbackDataProvider()
            {
                const string callbackUrlBase = "http://fci/complete/";

                var resourceProviderMock = new Mock<IResourceProvider>();
                resourceProviderMock.SetupGet(mock => mock.Nickname).Returns(Task.FromResult("nickname"));

                var guidGenerator = new Mock<IGuidGenerator>();
                guidGenerator.Setup(mock => mock.GenerateGuid()).Returns(_trackingKey);

                _workItemsApiMock = new Mock<IWorkItemsApi>();
                _workItemsApiMock.Setup(mock => mock.CreateWorkItemAsync(
                        It.Is<WorkItem>(wi => wi.ActivityId.Equals("nickname.activityId+activityLabel")
                                              && ((XrefTreeArgument) wi.Arguments["onComplete"]).Url.StartsWith(
                                                  callbackUrlBase)),
                        null, null, true))
                    .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                    {
                        Status = Status.Success
                    })));

                _publisher = new Publisher(
                    null,
                    new SerilogLoggerFactory().CreateLogger<Publisher>(),
                    resourceProviderMock.Object,
                    new Mock<IPostProcessing>().Object,
                    Options.Create(new PublisherConfiguration
                    {
                        CallbackUrlBase = callbackUrlBase,
                        CompletionCheck = CompletionCheck.Callback
                    }),
                    _workItemsApiMock.Object,
                    guidGenerator.Object);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    _publisher,
                    _trackingKey,
                    new WorkItemStatus
                    {
                        Status = Status.FailedLimitDataSize
                    },
                    _workItemsApiMock
                };
                yield return new object[]
                {
                    _publisher,
                    _trackingKey,
                    new WorkItemStatus
                    {
                        Status = Status.Cancelled
                    },
                    _workItemsApiMock
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(CallbackDataProvider))]
        public void RunWorkItemAsyncUsingCallback(Publisher publisher,
            string trackingKey, WorkItemStatus workItemStatus,
            Mock<IWorkItemsApi> workItemsApiMock)
        {
            var workItemTask = publisher.RunWorkItemAsync(_workItemArgs, _configMock.Object);
            publisher.NotifyTaskIsCompleted(trackingKey, workItemStatus);
            workItemTask.Wait();

            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(workItemStatus.Status, workItemTask.Result.Status);
            
            workItemsApiMock.Invocations.Clear();
        }
    }
}