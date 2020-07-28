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

        public ResourceProvider(IOptions<ForgeConfiguration> forgeConfigOptionsAccessor, DesignAutomationClient client, IConfiguration configuration, string bucketKey = null)
        {
            var forgeConfiguration = forgeConfigOptionsAccessor.Value.Validate();
            string suffix = configuration != null ? configuration.GetValue<string>("BucketKeySuffix") : "";
            BucketKey = bucketKey ?? $"projects-{forgeConfiguration.ClientId.Substring(0, 3)}-{forgeConfiguration.HashString()}{suffix}".ToLowerInvariant();

            _nickname = new Lazy<Task<string>>(async () => await client.GetNicknameAsync("me"));
        }
    }
}
