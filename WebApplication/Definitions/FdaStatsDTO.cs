/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
// ReSharper disable PossibleInvalidOperationException

namespace webapplication.Definitions
{
    public class FdaStatsDTO
    {
        private const double CreditsPerHour = 6.0; // should it be in config?
        private const double CreditsPerSecond = CreditsPerHour / 3600.0;

        public double Credits { get; set; }

        public double? Queueing { get; set; }
        public double? Download { get; set; }
        public double? Processing { get; set; }
        public double? Upload { get; set; }
        public double? Total { get; set; }

        /// <summary>
        /// Generate processing statistics.
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static FdaStatsDTO? All(ICollection<Statistics> stats)
        {
            return Convert(stats);
        }

        /// <summary>
        /// Generate processing statistics for cached item (without timings).
        /// </summary>
        public static FdaStatsDTO CreditsOnly(ICollection<Statistics> stats)
        {
            return All(stats).ClearTimings();
        }

        private static FdaStatsDTO? Convert(ICollection<Statistics> stats)
        {
            if (stats == null || stats.Count == 0) return null;

            var sum = new FdaStatsDTO();
            foreach (var s in stats)
            {
                var current = ConvertSingle(s);

                sum.Queueing = sum.Queueing.GetValueOrDefault() + current.Queueing;
                sum.Download = sum.Download.GetValueOrDefault() + current.Download;
                sum.Processing = sum.Processing.GetValueOrDefault() + current.Processing;
                sum.Upload = sum.Upload.GetValueOrDefault() + current.Upload;
                sum.Total = sum.Total.GetValueOrDefault() + current.Total;

                sum.Credits += current.Credits;
            }

            return sum;
        }

        private static FdaStatsDTO? ConvertSingle(Statistics stats)
        {
            // it's assumed that statistics calculated for successful job, so all timings are present
            var downloadSec = stats.TimeInstructionsStarted.Value.Subtract(stats.TimeDownloadStarted.Value).TotalSeconds;
            var processingSec = stats.TimeInstructionsEnded.Value.Subtract(stats.TimeInstructionsStarted.Value).TotalSeconds;
            var uploadSec = stats.TimeUploadEnded.Value.Subtract(stats.TimeInstructionsEnded.Value).TotalSeconds;

            var result = new FdaStatsDTO
                            {
                                Queueing = stats.TimeDownloadStarted.Value.Subtract(stats.TimeQueued).TotalSeconds,
                                Download = downloadSec,
                                Processing = processingSec,
                                Upload = uploadSec,
                                Total = stats.TimeFinished.Value.Subtract(stats.TimeQueued).TotalSeconds,
                                Credits = (downloadSec + processingSec + uploadSec) * CreditsPerSecond
                            };

            return result;
        }

        /// <summary>
        /// Remove timings.
        /// Used for cached jobs, where timings are not important.
        /// </summary>
        private FdaStatsDTO ClearTimings()
        {
            Queueing = null;
            Download = null;
            Processing = null;
            Upload = null;
            Total = null;

            return this;
        }
    }
}
