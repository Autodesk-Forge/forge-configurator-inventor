using System.Collections;
using System.Collections.Generic;
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

        private class TestDataProvider : IEnumerable<object[]>
        {
            private readonly Publisher _publisherCallback, _publisherPolling;
            private readonly Mock<IResourceProvider> _resourceProviderMock = new Mock<IResourceProvider>();
            private readonly Mock<IGuidGenerator> _guidGenerator = new Mock<IGuidGenerator>();

            private Mock<IWorkItemsApi> _workItemsApiMockCallback, _workItemsApiMockPolling;

            private const string TrackingKey = "cd26ccf675d64521884f1693c62ed303";
            private const string CallbackUrlBase = "http://fci/complete/";

            public TestDataProvider()
            {
                _resourceProviderMock.SetupGet(mock => mock.Nickname).Returns(Task.FromResult("nickname"));
                _guidGenerator.Setup(mock => mock.GenerateGuid()).Returns(TrackingKey);

                InitializeCallbackMocks();
                InitializePollingMocks();

                _publisherCallback = InitializePublisherMock(_workItemsApiMockCallback, _resourceProviderMock,
                    CallbackUrlBase,
                    _guidGenerator.Object, CompletionCheck.Callback);
                _publisherPolling = InitializePublisherMock(_workItemsApiMockPolling, _resourceProviderMock, null, null,
                    CompletionCheck.Polling);
            }

            private void InitializeCallbackMocks()
            {
                _workItemsApiMockCallback = new Mock<IWorkItemsApi>();
                _workItemsApiMockCallback.Setup(mock => mock.CreateWorkItemAsync(
                        It.Is<WorkItem>(wi => wi.ActivityId.Equals("nickname.activityId+activityLabel")
                                              && ((XrefTreeArgument) wi.Arguments["onComplete"]).Url.StartsWith(
                                                  CallbackUrlBase)),
                        null, null, true))
                    .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                    {
                        Status = Status.Pending
                    })));
            }

            private void InitializePollingMocks()
            {
                const string statusId = "id";

                _workItemsApiMockPolling = new Mock<IWorkItemsApi>();
                _workItemsApiMockPolling.Setup(mock => mock.CreateWorkItemAsync(
                        It.Is<WorkItem>(wi => wi.ActivityId.Equals("nickname.activityId+activityLabel")),
                        null, null, true))
                    .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                    {
                        Id = statusId,
                        Status = Status.Pending
                    })));

                _workItemsApiMockPolling.SetupSequence(mock => mock.GetWorkitemStatusAsync(
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
            }

            private Publisher InitializePublisherMock(IMock<IWorkItemsApi> workItemsApiMock,
                IMock<IResourceProvider> resourceProviderMock,
                string callbackUrlBase, IGuidGenerator guidGenerator, CompletionCheck completionCheck)
            {
                return new Publisher(
                    null,
                    new NullLogger<Publisher>(),
                    resourceProviderMock.Object,
                    new Mock<IPostProcessing>().Object,
                    Options.Create(new PublisherConfiguration
                    {
                        CallbackUrlBase = callbackUrlBase,
                        CompletionCheck = completionCheck
                    }),
                    workItemsApiMock.Object,
                    guidGenerator,
                    new Mock<ITaskUtil>().Object);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    _publisherCallback,
                    TrackingKey,
                    new WorkItemStatus
                    {
                        Status = Status.FailedLimitDataSize
                    },
                    _workItemsApiMockCallback
                };
                yield return new object[]
                {
                    _publisherCallback,
                    TrackingKey,
                    new WorkItemStatus
                    {
                        Status = Status.Cancelled
                    },
                    _workItemsApiMockCallback
                };
                yield return new object[]
                {
                    _publisherPolling,
                    null,
                    new WorkItemStatus
                    {
                        Status = Status.FailedInstructions
                    },
                    _workItemsApiMockPolling
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestDataProvider))]
        public void RunWorkItemAsyncUsingCallback(Publisher publisher, string trackingKey,
            WorkItemStatus workItemStatus, Mock<IWorkItemsApi> workItemsApiMock)
        {
            workItemsApiMock.Invocations.Clear();

            var workItemTask = publisher.RunWorkItemAsync(_workItemArgs, _configMock.Object);
            if (trackingKey != null) //in order to have single test for callback and polling
            {
                publisher.NotifyTaskIsCompleted(trackingKey, workItemStatus);
            }

            workItemTask.Wait();

            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(workItemStatus.Status, workItemTask.Result.Status);
        }
    }
}