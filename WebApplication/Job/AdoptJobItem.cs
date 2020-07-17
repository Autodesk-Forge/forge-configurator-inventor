using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
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
            var bucket = await _userResolver.GetBucketAsync(true);
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(_projectInfo.Name);

            string ossSourceModel = projectStorage.Project.OSSSourceModel;
            await bucket.SmartUploadAsync(_fileName, ossSourceModel);

            // cleanup before adoption
            File.Delete(_fileName);

            // adopt the project
            bool adopted = false;
            try
            {
                string signedUrl = await bucket.CreateSignedUrlAsync(ossSourceModel);
                await ProjectWork.AdoptAsync(_projectInfo, signedUrl);

                adopted = true;
            }
            catch (FdaProcessingException fpe)
            {
                var result = new ResultDTO
                {
                    Success = false,
                    Message = fpe.Message,
                    ReportUrl = fpe.ReportUrl
                };
                await resultSender.SendErrorAsync(result);
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