using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using WebApplication.Processing;
using WebApplication.State;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Job
{
    internal class AdoptJobItem : JobItemBase
    {
        private readonly string _packageId;
        private readonly UserResolver _userResolver;
        private readonly Uploads _uploads;
        private readonly DtoGenerator _dtoGenerator;

        public AdoptJobItem(ILogger logger, string packageId, ProjectWork projectWork, UserResolver userResolver, Uploads uploads, DtoGenerator dtoGenerator)
            : base(logger, null, projectWork)
        {
            _packageId = packageId;
            _userResolver = userResolver;
            _uploads = uploads;
            _dtoGenerator = dtoGenerator;
        }

        public override async Task ProcessJobAsync(IResultSender resultSender)
        {
            using var scope = Logger.BeginScope("Project Adoption ({Id})");

            Logger.LogInformation($"ProcessJob (Adopt) {Id} for package {_packageId} started.");

            // get upload information
            (ProjectInfo projectInfo, string fileName) = _uploads.GetUploadData(_packageId);
            
            // upload the file to OSS
            var bucket = await _userResolver.GetBucketAsync(true);
            ProjectStorage projectStorage = await _userResolver.GetProjectStorageAsync(projectInfo.Name);

            string ossSourceModel = projectStorage.Project.OSSSourceModel;
            await bucket.SmartUploadAsync(fileName, ossSourceModel);

            // adopt the project
            bool adopted = false;
            try
            {
                string signedUrl = await bucket.CreateSignedUrlAsync(ossSourceModel);
                await ProjectWork.AdoptAsync(projectInfo, signedUrl);

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

            Logger.LogInformation($"ProcessJob (Adopt) {Id} for package {_packageId} completed.");
            await resultSender.SendSuccessAsync(_dtoGenerator.ToDTO(projectStorage));
        }
    }
}