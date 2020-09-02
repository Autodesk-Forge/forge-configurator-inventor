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

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApplication.Processing;
using WebApplication.State;
using WebApplication.Definitions;
using WebApplication.Utilities;
using System.IO;

namespace WebApplication.Job
{
    internal class AdoptJobItem : JobItemBase
    {
        private readonly ProjectInfo _projectInfo;
        private readonly string _fileName;
        private readonly UserResolver _userResolver;
        private readonly DtoGenerator _dtoGenerator;

        public AdoptJobItem(ILogger logger, ProjectInfo projectInfo, string fileName, ProjectWork projectWork, UserResolver userResolver, DtoGenerator dtoGenerator)
            : base(logger, null, projectWork)
        {
            _projectInfo = projectInfo;
            _fileName = fileName;
            _userResolver = userResolver;
            _dtoGenerator = dtoGenerator;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            using var scope = Logger.BeginScope("Project Adoption ({Id})");

            Logger.LogInformation($"ProcessJob (Adopt) {Id} for project {_projectInfo.Name} started.");
            
            // upload the file to OSS
            var bucket = await _userResolver.GetBucketAsync(tryToCreate: true);
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(_projectInfo.Name);

            string ossSourceModel = projectStorage.Project.OSSSourceModel;

            await bucket.SmartUploadAsync(_fileName, ossSourceModel);

            // cleanup before adoption
            File.Delete(_fileName);

            // adopt the project
            bool adopted = false;
            try
            {
                string signedUploadedUrl = await bucket.CreateSignedUrlAsync(ossSourceModel);
                
                await ProjectWork.AdoptAsync(_projectInfo, signedUploadedUrl);

                adopted = true;
            }
            catch (FdaProcessingException fpe)
            {
                await resultSender.SendErrorAsync(Id, fpe.ReportUrl);
                return;
            }
            finally
            {
                // on any failure during adoption we consider that project adoption failed and it's not usable
                if (!adopted)
                {
                    Logger.LogInformation($"Adoption failed. Removing '{ossSourceModel}' OSS object.");
                    await bucket.DeleteObjectAsync(ossSourceModel);
                }
            }

            Logger.LogInformation($"ProcessJob (Adopt) {Id} for project {_projectInfo.Name} completed.");
            await resultSender.SendSuccessAsync(_dtoGenerator.ToDTO(projectStorage));
        }
    }
}