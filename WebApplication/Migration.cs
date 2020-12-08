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
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using WebApplication.Definitions;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;
using static MigrationApp.MigrationJob;

namespace MigrationApp
{
   public class Migration
   {
      private readonly IConfiguration _configuration;
      private readonly BucketPrefixProvider _bucketPrefix;
      private readonly IForgeOSS _forgeOSS;
      private readonly IResourceProvider _resourceProvider;
      private readonly ILogger<Migration> _logger;
      private readonly OssBucketFactory _bucketFactory;
      private readonly IServiceScopeFactory _serviceScopeFactory;

      public Migration(IConfiguration configuration, BucketPrefixProvider bucketPrefix, IForgeOSS forgeOSS, ILogger<Migration> logger, IResourceProvider resourceProvider, OssBucketFactory bucketFactory, IServiceProvider serviceProvider)
      {
         _forgeOSS = forgeOSS;
         _configuration = configuration;
         _bucketPrefix = bucketPrefix;
         _logger = logger;
         _resourceProvider = resourceProvider;
         _bucketFactory = bucketFactory;
         _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
      }
      public async Task ScanBuckets(List<MigrationJob> adoptJobs, List<MigrationJob> configJobs)
      {
         string suffixFrom = _configuration.GetValue<string>("BucketKeySuffixOld");
         string bucketKeyStart = _bucketPrefix.GetBucketPrefix(suffixFrom);
         List<string> bucketKeys = await _forgeOSS.GetBucketsAsync();
         string AnonymousBucketKeyOld = _resourceProvider.AnonymousBucketKey(suffixFrom);
         foreach (string bucketKey in bucketKeys)
         {
            if (bucketKey.StartsWith(bucketKeyStart) ||
                bucketKey == AnonymousBucketKeyOld
               )
            {
               await ScanBucket(adoptJobs, configJobs, bucketKey);
            }
         }
      }

      private async Task ScanBucket(List<MigrationJob> adoptJobs, List<MigrationJob> configJobs, string bucketKeyOld)
      {
         MigrationBucketKeyProvider bucketProvider;
         ProjectService projectService;
         using (var scope = _serviceScopeFactory.CreateScope())
         {
            bucketProvider = scope.ServiceProvider.GetService<MigrationBucketKeyProvider>();
            projectService = scope.ServiceProvider.GetService<ProjectService>();
         }

         OssBucket bucketOld = _bucketFactory.CreateBucket(bucketKeyOld);
         OssBucket bucketNew = _bucketFactory.CreateBucket(bucketProvider.GetBucketKeyFromOld(bucketKeyOld));

         List<string> projectNamesNew = new List<string>();
         try
         {
            List<string> projectNamesNewFromOss = (List<string>) await projectService.GetProjectNamesAsync(bucketNew);
            foreach (string projectName in projectNamesNewFromOss)
            {
               var ossAttributes = new OssAttributes(projectName);
               string metadataFile = ossAttributes.Metadata;
               // if metadata file is missing for project we consider that project not migrated
               if (await bucketNew.ObjectExistsAsync(metadataFile))
                  projectNamesNew.Add(projectName);
            }
         }
         catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
         {
            // swallow non existing item
         }

         // gather list of cache paramters files from the new bucket
         List<string> configKeysNew = new List<string>();
         try {
            List<ObjectDetails> configODsNew = await bucketNew.GetObjectsAsync($"{ONC.CacheFolder}/");
            foreach (ObjectDetails configODNew in configODsNew)
            {
               if (configODNew.ObjectKey.EndsWith(WebApplication.Utilities.LocalName.Parameters))
                  configKeysNew.Add(configODNew.ObjectKey);
            }
         }
         catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
         {
            // swallow non existing item
         }

         // gather projects to migrate
         List<string> projectNamesOld = (List<string>) await projectService.GetProjectNamesAsync(bucketOld);
         foreach (string projectName in projectNamesOld)
         {
            if (!projectNamesNew.Contains(projectName))
            {
               // new project list does not contain old project => lets copy and adopt
               var ossAttributes = new OssAttributes(projectName);
               string metadataFile = ossAttributes.Metadata;
               try {
                  ProjectMetadata projectMetadata = await bucketOld.DeserializeAsync<ProjectMetadata>(metadataFile);
                  ProjectInfo projectInfo = new ProjectInfo();
                  projectInfo.Name = projectName;
                  projectInfo.TopLevelAssembly = projectMetadata.TLA;

                  MigrationJob migrationJob;
                  using (var scope = _serviceScopeFactory.CreateScope())
                  {
                     migrationJob = scope.ServiceProvider.GetService<MigrationJob>();
                  }
                  migrationJob.SetJob(JobType.CopyAndAdopt, bucketOld, projectInfo, ONC.ProjectUrl(projectName));
                  adoptJobs.Add(migrationJob);
               } catch(Exception e)
               {
                  _logger.LogError(e, $"Project {projectName} in bucket {bucketKeyOld} does not have metadata file. Skipping it.");
               }
            }

            // process cached configurations
            List<ObjectDetails> configODs = await bucketOld.GetObjectsAsync($"{ONC.CacheFolder}/{projectName}/");
            foreach (ObjectDetails configOD in configODs)
            {
               string configKey = configOD.ObjectKey;
               if (configKey.EndsWith(WebApplication.Utilities.LocalName.Parameters))
               {
                  if (! configKeysNew.Contains(configKey))
                  {
                     InventorParameters parameters = await bucketOld.DeserializeAsync<InventorParameters>(configKey);;
                     ProjectInfo projectInfo = new ProjectInfo();
                     projectInfo.Name = projectName;

                     MigrationJob migrationJob;
                     using (var scope = _serviceScopeFactory.CreateScope())
                     {
                        migrationJob = scope.ServiceProvider.GetService<MigrationJob>();
                     }
                     migrationJob.SetJob(JobType.GenerateConfiguration, bucketOld, projectInfo, ONC.ProjectUrl(projectName), parameters);
                     configJobs.Add(migrationJob);
                  }
               }
            }
         }

         // check if any of migrated projects were deleted in old bucket
         // (user deleted them after migration started)
         foreach (string projectName in projectNamesNew)
         {
            if (!projectNamesOld.Contains(projectName))
            {
               MigrationJob migrationJob;
               using (var scope = _serviceScopeFactory.CreateScope())
               {
                  migrationJob = scope.ServiceProvider.GetService<MigrationJob>();
               }
               migrationJob.SetJob(JobType.RemoveNew, bucketNew, new ProjectInfo(projectName), null);
               adoptJobs.Add(migrationJob);
            }
         }
      }

      private async Task ProcessJobs(List<MigrationJob> jobs)
      {
         int taskIndex = 0;
         int parallelCount = Int16.Parse(_configuration.GetValue<string>("MigrationParallelCount") ?? "15");
         int jobIndex = 0;
         Task[] tasks = new Task[parallelCount];
         for (int i = 0; i < parallelCount; i++)
            tasks[i] = Task.CompletedTask;

         while (jobIndex < jobs.Count)
         {
            if (tasks[taskIndex].IsCompleted)
            {
               MigrationJob job = jobs[jobIndex]; 

               tasks[taskIndex] = job.Process();

               jobIndex++;
            }

            await Task.Delay(1000);
            taskIndex = (taskIndex + 1) % parallelCount;
         }

         // wait for completion of all tasks
         await Task.WhenAll(tasks);
      }

      public async Task Migrate(List<MigrationJob> adoptJobs, List<MigrationJob> configJobs)
      {
         await ProcessJobs(adoptJobs);
         await ProcessJobs(configJobs);
      }
   }
}
