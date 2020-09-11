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
using WebApplication.State;

namespace MigrationApp
{
   public class MigrationJob
   {
      public enum JobType {CopyAndAdopt};
      public JobType jobType;
      public OssBucket bucketOld;
      public ProjectInfo projectInfo;
      public string projectUrl;
      public MigrationJob(JobType jobTypeParam, OssBucket bucketOldParam, ProjectInfo projectInfoParam, string projectUrlParam)
      {
         jobType = jobTypeParam;
         bucketOld = bucketOldParam;
         projectInfo = projectInfoParam;
         projectUrl = projectUrlParam;
      }
   }
}