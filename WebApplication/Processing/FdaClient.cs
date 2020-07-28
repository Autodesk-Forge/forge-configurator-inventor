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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class FdaClient
    {
        private readonly TransferData _transferData;
        private readonly CreateSVF _svfWork;
        private readonly CreateSAT _satWork;
        private readonly CreateRFA _rfaWork;
        private readonly CreateThumbnail _thumbnailWork;
        private readonly ExtractParameters _parametersWork;
        private readonly AdoptProject _adoptWork;
        private readonly UpdateProject _updateProjectWork;
        private readonly AppBundleZipPaths _paths;
        private readonly UpdateParameters _updateParametersWork;
        private readonly ILogger<FdaClient> _logger;

        public FdaClient(Publisher publisher, IOptions<AppBundleZipPaths> appBundleZipPathsOptionsAccessor, ILogger<FdaClient> logger)
        {
            _transferData = new TransferData(publisher);
            _svfWork = new CreateSVF(publisher);
            _satWork = new CreateSAT(publisher);
            _rfaWork = new CreateRFA(publisher);
            _thumbnailWork = new CreateThumbnail(publisher);
            _parametersWork = new ExtractParameters(publisher);
            _adoptWork = new AdoptProject(publisher);
            _updateParametersWork = new UpdateParameters(publisher);
            _updateProjectWork = new UpdateProject(publisher);
            _paths = appBundleZipPathsOptionsAccessor.Value;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // create bundles and activities
            await _transferData.InitializeAsync(_paths.EmptyExe);
            await _svfWork.InitializeAsync(_paths.CreateSVF);
            await _satWork.InitializeAsync(_paths.CreateSAT);
            await _rfaWork.InitializeAsync(_paths.CreateRFA);
            await _thumbnailWork.InitializeAsync(_paths.CreateThumbnail);
            await _parametersWork.InitializeAsync(_paths.ExtractParameters);
            await _updateParametersWork.InitializeAsync(_paths.UpdateParameters);
            await _adoptWork.InitializeAsync(null /* does not matter */);
            await _updateProjectWork.InitializeAsync(null /* does not matter */);
        }

        public async Task CleanUpAsync()
        {
            // delete bundles and activities
            await _transferData.CleanUpAsync();
            await _svfWork.CleanUpAsync();
            await _satWork.CleanUpAsync();
            await _rfaWork.CleanUpAsync();
            await _thumbnailWork.CleanUpAsync();
            await _parametersWork.CleanUpAsync();
            await _updateParametersWork.CleanUpAsync();
            await _adoptWork.CleanUpAsync();
            await _updateProjectWork.CleanUpAsync();
        }

        public Task<ProcessingResult> AdoptAsync(AdoptionData projectData)
        {
            return _adoptWork.ProcessAsync(projectData);
        }

        public Task<ProcessingResult> UpdateAsync(UpdateData projectData)
        {
            return _updateProjectWork.ProcessAsync(projectData);
        }

        internal Task<ProcessingResult> TransferAsync(string source, string target)
        {
            return _transferData.ProcessAsync(source, target);
        }

        internal async Task<ProcessingResult> GenerateRfa(ProcessingArgs satData, ProcessingArgs rfaData)
        {
            ProcessingResult satResult = await _satWork.ProcessAsync(satData);
            if (!satResult.Success)
            {
                satResult.ErrorMessage = "Failed to generate SAT file";
                return satResult;
            }

            ProcessingResult rfaResult = await _rfaWork.ProcessAsync(rfaData);
            if (!rfaResult.Success)
            {
                rfaResult.ErrorMessage = "Failed to generate RFA file";
                return rfaResult;
            }

            return rfaResult;
        }
    }
}
