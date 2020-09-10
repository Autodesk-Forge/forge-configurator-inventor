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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.Middleware;
using WebApplication.Services;
using WebApplication.Utilities;

namespace WebApplication.State
{
    /// <summary>
    /// Business logic to differentiate state for logged in and anonymous users.
    /// Lifetime: scoped.
    /// </summary>
    public class UserResolver
    {
        private readonly IForgeOSS _forgeOSS;
        private readonly LocalCache _localCache;
        private readonly ILogger<UserResolver> _logger;
        private readonly IBucketKeyProvider _bucketKeyProvider;
        private readonly ProfileProvider _profileProvider;

        /// <summary>
        /// OSS bucket for anonymous user.
        /// </summary>
        public OssBucket AnonymousBucket { get; }

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS, IBucketKeyProvider bucketKeyProvider,
                            LocalCache localCache, ILogger<UserResolver> logger, ProfileProvider profileProvider)
        {
            _forgeOSS = forgeOSS;
            _localCache = localCache;
            _logger = logger;
            _bucketKeyProvider = bucketKeyProvider;
            _profileProvider = profileProvider;

            AnonymousBucket = new OssBucket(_forgeOSS, _bucketKeyProvider.AnonymousBucketKey, logger);
        }

        public async Task<OssBucket> GetBucketAsync(bool tryToCreate = false)
        {
            var bucket = new OssBucket(_forgeOSS, await _bucketKeyProvider.GetBucketKeyAsync(), _logger);
            if (tryToCreate)
            {
                // TODO: VERY INEFFECTIVE!!!!!
                try
                {
                    await bucket.CreateAsync();
                }
                catch (ApiException e) when(e.ErrorCode == StatusCodes.Status409Conflict)
                {
                    // means - the bucket already exists
                }
            }

            return bucket;
        }

        public async Task<ICollection<string>> GetProjectNames()
        {
            OssBucket bucket = await GetBucketAsync();
            return (await bucket.GetObjectsAsync(ONC.ProjectsMask))
                                .Select(objDetails =>  ONC.ToProjectName(objDetails.ObjectKey))
                                .ToList();
        }

        public async Task<Project> GetProjectAsync(string projectName, bool ensureDir = true)
        {
            var userFullDirName = await GetFullUserDir();
            if (ensureDir)
            {
                Directory.CreateDirectory(userFullDirName); // TODO: should not do it each time
            }
            return new Project(projectName, userFullDirName);
        }

        /// <summary>
        /// Get project storage by project name.
        /// </summary>
        public async Task<ProjectStorage> GetProjectStorageAsync(string projectName, bool ensureDir = true)
        {
            var project = await GetProjectAsync(projectName, ensureDir);
            return new ProjectStorage(project);
        }


        public async Task<string> GetFullUserDir()
        {
            string userDir;
            if (_profileProvider.IsAuthenticated)
            {
                var profile = await _profileProvider.GetProfileAsync();

                // generate dirname to hide Oxygen user ID
                userDir = Crypto.GenerateHashString("SDRA" + profile.userId);
            }
            else
            {
                userDir = "_anonymous";
            }

            return Path.Combine(_localCache.LocalRootName, userDir);
        }

        /// <summary>
        /// Ensure that the file exists in local cache for a project.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="fileName">Short file name.</param>
        /// <param name="hash">Parameters hash (default is used if not provided).</param>
        /// <returns>Full path to the existing local file.</returns>
        public async Task<string> EnsureLocalFile(string projectName, string fileName, string hash = null)
        {
            var projectStorage = await GetProjectStorageAsync(projectName);

            var localNames = projectStorage.GetLocalNames(hash);
            var fullLocalName = localNames.ToFullName(fileName);

            if (! File.Exists(fullLocalName))
            {
                _logger.LogInformation($"Restoring missing '{fullLocalName}' for '{projectName}'");

                Directory.CreateDirectory(localNames.BaseDir);

                var bucket = await GetBucketAsync();
                await bucket.DownloadFileAsync(projectStorage.GetOssNames(hash).ToFullName(fileName), fullLocalName);
            }

            return fullLocalName;
        }
    }
}
