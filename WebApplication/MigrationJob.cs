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

using WebApplication.Definitions;

namespace MigrationApp
{
   public class MigrationJob
   {
      public enum JobType {CopyAndAdopt};
      public JobType jobType;
      public string bucketKey;
      public ProjectInfo projectInfo;
      public string projectUrl;
      public MigrationJob(JobType jobTypeParam, string bucketKeyParam, ProjectInfo projectInfoParam, string projectUrlParam)
      {
         jobType = jobTypeParam;
         bucketKey = bucketKeyParam;
         projectInfo = projectInfoParam;
         projectUrl = projectUrlParam;
      }
   }
}