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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared;
using webapplication.Definitions;
using webapplication.Processing;
using webapplication.Services;
using webapplication.State;

namespace MigrationApp
{
   public class MigrationJob
   {
      private readonly MigrationBucketKeyProvider _bucketProvider;
      private readonly UserResolver _userResolver;
      private readonly ProjectWork _projectWork;
      private readonly ILogger<MigrationJob> _logger;
      private readonly ProjectService _projectService;

      public enum JobType {CopyAndAdopt, RemoveNew, GenerateConfiguration };
      private JobType jobType;
      private OssBucket bucket;
      private ProjectInfo projectInfo;
      private string projectUrl;
      private InventorParameters parameters;
      public MigrationJob(MigrationBucketKeyProvider bucketProvider, UserResolver userResolver, ProjectWork projectWork, ILogger<MigrationJob> logger, ProjectService projectService)
      {
         _bucketProvider = bucketProvider;
         _userResolver = userResolver;
         _projectWork = projectWork;
         _logger = logger;
         _projectService = projectService;
      }
      public void SetJob(JobType jobTypeParam, OssBucket bucketParam, ProjectInfo projectInfoParam, string projectUrlParam, InventorParameters? parametersParam = null)
      {
         jobType = jobTypeParam;
         bucket = bucketParam;
         projectInfo = projectInfoParam;
         projectUrl = projectUrlParam;
         parameters = parametersParam;
      }

      private async Task GenerateConfiguration(string logId)
      {
         _bucketProvider.SetBucketKeyFromOld(bucket.BucketKey);
         _logger.LogInformation($"{logId}: Generating config for project {projectInfo.Name} in bucket {bucket.BucketKey}");

         OssBucket bucketNew = await _userResolver.GetBucketAsync();
         var storage = await _userResolver.GetProjectStorageAsync(projectInfo.Name);
         await storage.EnsureAttributesAsync(bucketNew);

         try
         {
            await _projectWork.DoSmartUpdateAsync(parameters, projectInfo.Name);
            _logger.LogInformation($"{logId}: Configuration {parameters} for project {projectInfo.Name} was generated.");
         }
         catch(Exception e)
         {
            _logger.LogError(e, $"{logId}: Configuration {parameters} for project {projectInfo.Name} was NOT generated.");
         }

         return;
      }

      private async Task RemoveNew(string logId)
      {
         _logger.LogInformation($"{logId}: Deleting project {projectInfo.Name} from bucket {bucket.BucketKey}");
         List<string> projectList = new List<string>() {projectInfo.Name};
         await _projectService.DeleteProjects(projectList, bucket);
      }

      private async Task CopyAndAdopt(string logId)
      {
         _bucketProvider.SetBucketKeyFromOld(bucket.BucketKey);
         _logger.LogInformation($"{logId}: Processing new project {projectInfo.Name} in bucket {bucket.BucketKey}");
         OssBucket bucketNew = await _userResolver.GetBucketAsync(true);

         string signedUrlOld = await bucket.CreateSignedUrlAsync(projectUrl, ObjectAccess.Read);
         string signedUrlNew = await bucketNew.CreateSignedUrlAsync(projectUrl, ObjectAccess.ReadWrite);

         try
         {
            await _projectWork.FileTransferAsync(signedUrlOld, signedUrlNew);
         }
         catch(Exception e)
         {
            _logger.LogError(e, $"{logId}: Project {projectInfo.Name} cannot be copied.");
            return;
         }

         try
         {
            await _projectWork.AdoptAsync(projectInfo, signedUrlNew);
            _logger.LogInformation($"{logId}: Project {projectInfo.Name} was adopted");
         }
         catch(Exception e)
         {
            _logger.LogError(e, $"{logId}: Project {projectInfo.Name} was not adopted");
         }
      }

      public async Task Process(string logId)
      {
         switch (jobType)
         {
            case JobType.CopyAndAdopt:
               await CopyAndAdopt(logId);
               break;
            case JobType.RemoveNew:
               await RemoveNew(logId);
               break;
            case JobType.GenerateConfiguration:
               await GenerateConfiguration(logId);
               break;
         }
      }
   }
}