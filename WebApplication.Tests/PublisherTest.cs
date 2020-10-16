using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
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
                        Status = Status.Pending
                    })));

                _publisher = new Publisher(
                    null,
                    new NullLogger<Publisher>(),
                    resourceProviderMock.Object,
                    new Mock<IPostProcessing>().Object,
                    Options.Create(new PublisherConfiguration
                    {
                        CallbackUrlBase = callbackUrlBase,
                        CompletionCheck = CompletionCheck.Callback
                    }),
                    _workItemsApiMock.Object,
                    guidGenerator.Object,
                    null);
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

        private class PollingDataProvider : IEnumerable<object[]>
        {
            private readonly Publisher _publisher;

            private readonly Mock<IWorkItemsApi> _workItemsApiMock;

            public PollingDataProvider()
            {
                const string statusId = "id";
                
                var resourceProviderMock = new Mock<IResourceProvider>();
                resourceProviderMock.SetupGet(mock => mock.Nickname).Returns(Task.FromResult("nickname"));

                _workItemsApiMock = new Mock<IWorkItemsApi>();
                _workItemsApiMock.Setup(mock => mock.CreateWorkItemAsync(
                        It.Is<WorkItem>(wi => wi.ActivityId.Equals("nickname.activityId+activityLabel")),
                        null, null, true))
                    .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                    {
                        Id = statusId,
                        Status = Status.Pending
                    })));

                _workItemsApiMock.SetupSequence(mock => mock.GetWorkitemStatusAsync(
                        statusId, null, null, true))
                    .Returns(PrepareWorkItemStatusResult(Status.Pending))
                    .Returns(PrepareWorkItemStatusResult(Status.Pending))
                    .Returns(PrepareWorkItemStatusResult(Status.Inprogress))
                    .Returns(PrepareWorkItemStatusResult(Status.Inprogress))
                    .Returns(PrepareWorkItemStatusResult(Status.FailedInstructions));

                Task<ApiResponse<WorkItemStatus>> PrepareWorkItemStatusResult(Status status)
                {
                    return Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                    {
                        Id = statusId,
                        Status = status
                    }));
                }

                _publisher = new Publisher(
                    null,
                    new NullLogger<Publisher>(),
                    resourceProviderMock.Object,
                    new Mock<IPostProcessing>().Object,
                    Options.Create(new PublisherConfiguration
                    {
                        CompletionCheck = CompletionCheck.Polling
                    }),
                    _workItemsApiMock.Object,
                    null,
                    new Mock<ITaskUtil>().Object);
            }
            
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    _publisher,
                    new WorkItemStatus
                    {
                        Status = Status.FailedInstructions
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
            workItemsApiMock.Invocations.Clear();
            
            var workItemTask = publisher.RunWorkItemAsync(_workItemArgs, _configMock.Object);
            publisher.NotifyTaskIsCompleted(trackingKey, workItemStatus);
            workItemTask.Wait();

            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(workItemStatus.Status, workItemTask.Result.Status);
            
            workItemsApiMock.Invocations.Clear();
        }
        
        [Theory]
        [ClassData(typeof(PollingDataProvider))]
        public void RunWorkItemAsyncUsingPolling(Publisher publisher, WorkItemStatus workItemStatus,
            Mock<IWorkItemsApi> workItemsApiMock)
        {
            workItemsApiMock.Invocations.Clear();

            var workItemTask = publisher.RunWorkItemAsync(_workItemArgs, _configMock.Object);
            workItemTask.Wait();

            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(workItemStatus.Status, workItemTask.Result.Status);
            
        }
    }
}