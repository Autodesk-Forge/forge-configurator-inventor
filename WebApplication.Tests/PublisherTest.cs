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

        private readonly Mock<IResourceProvider> _resourceProviderMock = new();

        public PublisherTest()
        {
            _workItemArgs = new Dictionary<string, IArgument>();

            _configMock = new Mock<IForgeAppBase>();
            _configMock.Setup(mock => mock.ActivityId).Returns("activityId");
            _configMock.Setup(mock => mock.ActivityLabel).Returns("activityLabel");

            _resourceProviderMock.SetupGet(mock => mock.Nickname).Returns(Task.FromResult("nickname"));
        }

        private Mock<IWorkItemsApi> PrepareWorkItemsApiMockForCallback(string callbackUrlBase, Status status)
        {
            var workItemsApiMock = new Mock<IWorkItemsApi>();
            workItemsApiMock.Setup(mock => mock.CreateWorkItemAsync(
                    It.Is<WorkItem>(wi => wi.ActivityId.Equals("nickname.activityId+activityLabel")
                                          && ((XrefTreeArgument) wi.Arguments["onComplete"]).Url.StartsWith(
                                              callbackUrlBase)),
                    null, null, true))
                .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                {
                    Status = status
                })));
            return workItemsApiMock;
        }

        private Mock<IWorkItemsApi> PrepareWorkItemsApiMockForPolling(params Status[] statuses)
        {
            const string statusId = "id";

            var workItemsApiMock = new Mock<IWorkItemsApi>();

            workItemsApiMock.Setup(mock => mock.CreateWorkItemAsync(
                    It.Is<WorkItem>(wi => wi.ActivityId.Equals("nickname.activityId+activityLabel")),
                    null, null, true))
                .Returns(Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                {
                    Id = statusId,
                    Status = statuses[0]
                })));

            var workItemApiMockSetup =
                workItemsApiMock.SetupSequence(mock => mock.GetWorkitemStatusAsync(statusId, null, null, true));

            foreach (var status in statuses)
            {
                workItemApiMockSetup.Returns(PrepareWorkItemStatusResult(status));
            }

            Task<ApiResponse<WorkItemStatus>> PrepareWorkItemStatusResult(Status status)
            {
                return Task.FromResult(new ApiResponse<WorkItemStatus>(null, new WorkItemStatus
                {
                    Id = statusId,
                    Status = status
                }));
            }

            return workItemsApiMock;
        }

        private Publisher InitializePublisherMock(IMock<IWorkItemsApi> workItemsApiMock,
            IMock<IResourceProvider> resourceProviderMock, string callbackUrlBase, IGuidGenerator guidGenerator,
            CompletionCheck completionCheck)
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

        private class CallbackTestDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    "cd26ccf675d64521884f1693c62ed303",
                    Status.Pending,
                    Status.Success
                };
                yield return new object[]
                {
                    "344ce0a424d94bae9b7847c795b2c8fd",
                    Status.Pending,
                    Status.Cancelled
                };
                yield return new object[]
                {
                    "f27a4b420fff4581b5580e6658d5a58f",
                    Status.Inprogress,
                    Status.FailedLimitDataSize
                };
                yield return new object[]
                {
                    "709e6bbe6b4c4a5abc2322d72b3d6073",
                    Status.Inprogress,
                    Status.FailedDownload
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class PollingTestDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    Status.Pending,
                    Status.Pending,
                    Status.Inprogress,
                    Status.Inprogress,
                    Status.Inprogress,
                    Status.Success
                };
                yield return new object[]
                {
                    Status.Pending,
                    Status.Inprogress,
                    Status.Cancelled
                };
                yield return new object[]
                {
                    Status.Inprogress,
                    Status.Inprogress,
                    Status.Inprogress,
                    Status.Inprogress,
                    Status.FailedLimitDataSize
                };
                yield return new object[]
                {
                    Status.Inprogress,
                    Status.FailedDownload
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(CallbackTestDataProvider))]
        public void RunWorkItemAsyncUsingCallback(string trackingKey, Status initStatus, Status finalStatus)
        {
            //setup
            const string callbackUrlBase = "http://fci/complete/";

            var workItemsApiMock = PrepareWorkItemsApiMockForCallback(callbackUrlBase, initStatus);

            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(mock => mock.GenerateGuid()).Returns(trackingKey);

            var publisher = InitializePublisherMock(workItemsApiMock, _resourceProviderMock, callbackUrlBase,
                guidGenerator.Object, CompletionCheck.Callback);

            //when
            var workItemTask = publisher.RunWorkItemAsync(_workItemArgs, _configMock.Object);
            publisher.NotifyTaskIsCompleted(trackingKey, new WorkItemStatus {Status = finalStatus});
            workItemTask.Wait();

            //then
            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(finalStatus, workItemTask.Result.Status);
        }

        [Theory]
        [ClassData(typeof(PollingTestDataProvider))]
        public void RunWorkItemAsyncUsingPolling(params Status[] statuses)
        {
            //setup
            var workItemsApiMock = PrepareWorkItemsApiMockForPolling(statuses);

            var publisher = InitializePublisherMock(workItemsApiMock, _resourceProviderMock, null, null,
                CompletionCheck.Polling);

            //when
            var workItemTask = publisher.RunWorkItemAsync(_workItemArgs, _configMock.Object);
            workItemTask.Wait();

            //then
            workItemsApiMock.Verify(mock => mock.CreateWorkItemAsync(It.IsAny<WorkItem>(), null, null, true),
                Times.Once);
            Assert.Equal(statuses[^1], workItemTask.Result.Status);
        }
    }
}