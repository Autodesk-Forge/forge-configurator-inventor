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
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebApplication.Services;

namespace WebApplication.Utilities
{
    public class ResourceProvider
    {
        /// <summary>
        /// Bucket key for anonymous user.
        /// </summary>
        public string BucketKey { get; }

        public Task<string> Nickname => _nickname.Value;
        private readonly Lazy<Task<string>> _nickname;
        private readonly ForgeConfiguration _forgeConfiguration;
        private readonly BucketPrefixProvider _bucketPrefixProvider;
        private readonly IConfiguration _configuration;

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, IConfiguration configuration, BucketPrefixProvider bucketPrefixProvider, string bucketKey = null)
        {
            _forgeConfiguration = forgeConfigOptionsAccessor.Value.Validate();
            _configuration = configuration;
            _bucketPrefixProvider = bucketPrefixProvider;

            BucketKey = bucketKey ?? AnonymousBucketKey();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }

        public string AnonymousBucketKey(string suffixParam = null)
        {
            string suffix = (suffixParam == null) ? (_configuration != null) ? _configuration.GetValue<string>("BucketKeySuffix") : "" : suffixParam;
            return $"projects-{_forgeConfiguration.ClientId.Substring(0, 3)}-{_forgeConfiguration.HashString()}{suffix}".ToLowerInvariant();
        }

        public string LoggedUserBucketKey(string userId, string userHashParam = null)
        {
            // an OSS bucket must have a unique name, so it should be generated in a way,
            // so it a Forge user gets registered into several deployments it will not cause
            // name collisions. So use client ID (as a salt) to generate bucket name.
            var userHash = userHashParam == null ? Crypto.GenerateHashString(_forgeConfiguration.ClientId + userId) : userHashParam;
            return $"{_bucketPrefixProvider.GetBucketPrefix()}-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();
        }
    }
}
