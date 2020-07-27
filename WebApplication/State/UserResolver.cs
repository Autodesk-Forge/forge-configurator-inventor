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
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Middleware;
using WebApplication.Services;
using WebApplication.Utilities;

namespace WebApplication.State
{
    /// <summary>
    /// Business logic to differentiate state for logged in and anonymous users.
    /// </summary>
    public class UserResolver
    {
        private readonly IForgeOSS _forgeOSS;
        private readonly LocalCache _localCache;
        private readonly ILogger<UserResolver> _logger;
        private readonly ForgeConfiguration _forgeConfig;

        private readonly Lazy<Task<dynamic>> _lazyProfile;

        /// <summary>
        /// OSS bucket for anonymous user.
        /// </summary>
        public OssBucket AnonymousBucket { get; }

        public string Token { private get; set; }
        public bool IsAuthenticated => ! string.IsNullOrEmpty(Token);

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS,
                            IOptions<ForgeConfiguration> forgeConfiguration, LocalCache localCache, ILogger<UserResolver> logger)
        {
            _forgeOSS = forgeOSS;
            _localCache = localCache;
            _logger = logger;
            _forgeConfig = forgeConfiguration.Value;

            AnonymousBucket = new OssBucket(_forgeOSS, resourceProvider.BucketKey, logger);

            _lazyProfile = new Lazy<Task<dynamic>>(async () => await _forgeOSS.GetProfileAsync(Token));
        }

        public string GetBucketPrefix()
        {
            return $"authd-{_forgeConfig.ClientId}".ToLowerInvariant();
        }

        public async Task<OssBucket> GetBucketAsync(bool tryToCreate = false)
        {
            if (! IsAuthenticated) return AnonymousBucket;

            dynamic profile = await GetProfileAsync();
            var userId = profile.userId;

            // an OSS bucket must have a unique name, so it should be generated in a way,
            // so it a Forge user gets registered into several deployments it will not cause
            // name collisions. So use client ID (as a salt) to generate bucket name.
            var userHash = Crypto.GenerateHashString(_forgeConfig.ClientId + userId);
            var bucketKey = $"{GetBucketPrefix()}-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();

            var bucket = new OssBucket(_forgeOSS, bucketKey, _logger);
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

        public Task<dynamic> GetProfileAsync() => _lazyProfile.Value;

        public async Task<string> GetFullUserDir()
        {
            string userDir;
            if (IsAuthenticated)
            {
                var profile = await GetProfileAsync();

                // generate dirname to hide Oxygen user ID
                userDir = Crypto.GenerateHashString("SDRA" + profile.userId);
            }
            else
            {
                userDir = "_anonymous";
            }

            return Path.Combine(_localCache.LocalRootName, userDir);
        }
    }
}
