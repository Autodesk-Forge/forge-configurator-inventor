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

        [Fact]
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
            const int costPerHour = 6;
            var expected = paidTimeSec * costPerHour / 3600;
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
