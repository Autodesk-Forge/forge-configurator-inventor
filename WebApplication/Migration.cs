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
using Autodesk.Forge.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApplication.Definitions;
using WebApplication.Processing;
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
      private readonly MigrationBucketKeyProvider _bucketProvider;
      private readonly ResourceProvider _resourceProvider;
      private readonly UserResolver _userResolver;
      private readonly ProjectWork _projectWork;
      private readonly ILogger<Migration> _logger;

      public Migration(IConfiguration configuration, BucketPrefixProvider bucketPrefix, IForgeOSS forgeOSS, MigrationBucketKeyProvider bucketProvider, UserResolver userResolver, ProjectWork projectWork, ILogger<Migration> logger, ResourceProvider resourceProvider)
      {
         _forgeOSS = forgeOSS;
         _configuration = configuration;
         _bucketPrefix = bucketPrefix;
         _bucketProvider = bucketProvider;
         _userResolver = userResolver;
         _projectWork = projectWork;
         _logger = logger;
         _resourceProvider = resourceProvider;
      }
      public async Task<List<MigrationJob>> ScanBuckets()
      {
         List<MigrationJob> migrationJobs = new List<MigrationJob>();
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
               await ScanBucket(migrationJobs, bucketKey);
            }
         }

         return migrationJobs;
      }

      private async Task ScanBucket(List<MigrationJob> migrationJobs, string bucketKey)
      {
         List<ObjectDetails> ossProjectFiles = await _forgeOSS.GetBucketObjectsAsync(bucketKey, "projects/");
         foreach (ObjectDetails file in ossProjectFiles)
         {
            string projectUrl = file.ObjectKey;
            string projectName = projectUrl.Split('/')[1];
            string attributeFile = "attributes/" + projectName + "/metadata.json";
            string bucketKeyNew = _bucketProvider.SetBucketKeyFromOld(bucketKey);

            // check attributes file existance in new destination bucket
            if (await _forgeOSS.DoesObjectExist(bucketKeyNew, attributeFile))
               continue;

            await _forgeOSS.DownloadFileAsync(bucketKey, attributeFile, "metadata.json");
            ProjectMetadata projectMetadata = Json.DeserializeFile<ProjectMetadata>("metadata.json");

            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Name = projectName;
            projectInfo.TopLevelAssembly = projectMetadata.TLA;

            migrationJobs.Add(new MigrationJob(JobType.CopyAndAdopt, bucketKey, projectInfo, projectUrl));
         }
      }

      public async Task Migrate(List<MigrationJob> migrationJobs)
      {
         foreach (MigrationJob job in migrationJobs)
         {            
            _bucketProvider.SetBucketKeyFromOld(job.bucketKey);
            OssBucket bucketNew = await _userResolver.GetBucketAsync(true);

            string signedUrlOld = await _forgeOSS.CreateSignedUrlAsync(job.bucketKey, job.projectUrl, ObjectAccess.Read);
            string signedUrlNew = await bucketNew.CreateSignedUrlAsync(job.projectUrl, ObjectAccess.ReadWrite);

            try
            {
               await _projectWork.FileTransferAsync(signedUrlOld, signedUrlNew);
            }
            catch(Exception e)
            {
               _logger.LogError("Project " + job.projectInfo.Name + " cannot be copied\nException:" + e.Message);
               continue;
            }

            try
            {
               await _projectWork.AdoptAsync(job.projectInfo, signedUrlNew);
               _logger.LogInformation("Project " + job.projectInfo.Name + " was adopted");
            }
            catch(Exception e)
            {
               _logger.LogError("Project " + job.projectInfo.Name + " was not adopted\nException:" + e.Message);
            }
         }
      }

        /*
        public async Task<string> Refresh()
        {
            string returnValue = "";
            List<ObjectDetails> ossFiles = await _forgeOSS.GetBucketObjectsAsync(_bucket.BucketKey, "cache/");
            foreach (ObjectDetails file in ossFiles)
            {
                string[] fileParts = file.ObjectKey.Split('/');
                string project = fileParts[1];
                string hash = fileParts[2];
                string fileName = fileParts[3];
                if (fileName == "parameters.json")
                {
                    returnValue += "Project " + project + " (" + hash + ") is being updated\n";
                    string paramsFile = Path.Combine(_localCache.LocalRootName, "params.json");
                    await _bucket.DownloadFileAsync(file.ObjectKey, paramsFile);
                    InventorParameters inventorParameters = Json.DeserializeFile<InventorParameters>(paramsFile);
                    try
                    {
                        await _projectWork.DoSmartUpdateAsync(inventorParameters, project, true);
                        returnValue += "Project " + project + " (" + hash + ") was updated\n";
                    } catch(Exception e)
                    {
                        returnValue += "Project " + project + " (" + hash + ") update failed\nException: " + e.Message + "\n";
                    }
                }
            }

            return returnValue;
        }
        */
   }
}