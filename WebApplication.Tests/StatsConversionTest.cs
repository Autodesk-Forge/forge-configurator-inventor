using System;
using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;
using Xunit;
// ReSharper disable PossibleInvalidOperationException

namespace WebApplication.Tests
{
    public class StatsConversionTest
    {
        private const int CostPerHour = 6;

        [Fact]
        public void Null()
        {
            Assert.Null(FdaStatsDTO.All(null));
        }

        [Fact]
        public void Empty()
        {
            Assert.Null(FdaStatsDTO.All(new List<Statistics>()));
        }

        [Fact]
        public void InvalidInput()
        {
            // it's expected that statistics is for successful jobs.  Missing field(s) should throw an error.
            Assert.ThrowsAny<Exception>(() => FdaStatsDTO.All(new List<Statistics>(new []{ new Statistics() })));
        }

        [Fact(DisplayName = "Statistics for single work item")]
        public void Single()
        {
            var stats = MakeStat(downloadOffset: 15, instructionStartedOffset: 20, instructionEndedOffset: 40, uploadOffset: 50, finishedOffset: 52);

            var calculated = FdaStatsDTO.All(new List<Statistics>(new []{ stats }));
            Assert.Equal(15, calculated.Queueing.Value, 1);
            Assert.Equal(5, calculated.Download.Value, 1);
            Assert.Equal(20, calculated.Processing.Value, 1);
            Assert.Equal(10, calculated.Upload.Value, 1);
            Assert.Equal(52, calculated.Total.Value, 1);

            // validate credits
            const double paidTimeSec = 35.0; // download + processing + upload
            const double expected = paidTimeSec * CostPerHour / 3600;
            Assert.Equal(expected, calculated.Credits, 4);
        }

        [Fact(DisplayName = "Statistics for multiple work items")]
        public void Multiple()
        {
            var stats1 = MakeStat(downloadOffset: 15, instructionStartedOffset: 20, instructionEndedOffset: 40, uploadOffset: 50, finishedOffset: 52);
            var stats2 = MakeStat(downloadOffset: 1, instructionStartedOffset: 6, instructionEndedOffset: 22, uploadOffset: 25, finishedOffset: 33);

            var calculated = FdaStatsDTO.All(new List<Statistics>(new []{ stats1, stats2 }));
            Assert.Equal(16, calculated.Queueing.Value, 1);
            Assert.Equal(10, calculated.Download.Value, 1);
            Assert.Equal(36, calculated.Processing.Value, 1);
            Assert.Equal(13, calculated.Upload.Value, 1);
            Assert.Equal(85, calculated.Total.Value, 1);

            // validate credits
            const double paidTimeSec = 35.0 + 24.0; // download + processing + upload
            const double expected = paidTimeSec * CostPerHour / 3600;
            Assert.Equal(expected, calculated.Credits, 4);
        }

        private static Statistics MakeStat(double downloadOffset, double instructionStartedOffset, double instructionEndedOffset, double uploadOffset, double finishedOffset)
        {
            var startTime = DateTime.UtcNow;
            return new Statistics
                    {
                        TimeQueued = startTime,
                        TimeDownloadStarted = startTime.AddSeconds(downloadOffset),
                        TimeInstructionsStarted = startTime.AddSeconds(instructionStartedOffset),
                        TimeInstructionsEnded = startTime.AddSeconds(instructionEndedOffset),
                        TimeUploadEnded = startTime.AddSeconds(uploadOffset),
                        TimeFinished = startTime.AddSeconds(finishedOffset)
                    };
        }
    }
}
