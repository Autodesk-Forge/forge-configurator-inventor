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

namespace WebApplication.Definitions
{
    public class FdaStatsDTO
    {
        public double Credits { get; set; }
        public double? Download { get; set; }
        public double? Processing { get; set; }
        public double? Upload { get; set; }
        public double? Queueing { get; set; }

        public static FdaStatsDTO All(IEnumerable<Statistics> stats)
        {
            return new FdaStatsDTO
            {
                Credits = 3.14,
                Download = 0.1,
                Processing = 0.2,
                Upload = 0.3,
                Queueing = 0.4
            };
        }

        /// <summary>
        /// Generate stats for cached item (without timings).
        /// </summary>
        public static FdaStatsDTO CreditsOnly(IEnumerable<Statistics> stats)
        {
            return new FdaStatsDTO { Credits = 0.3 };
        }
    }
}
