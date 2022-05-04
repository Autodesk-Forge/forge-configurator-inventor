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
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using webapplication.Definitions;
using webapplication.Middleware;
using webapplication.Processing;
using webapplication.Services;
using webapplication.State;

namespace webapplication;

/// <summary>
/// Initialize data for anonymous user.
/// </summary>
public class Initializer
{
    private readonly ILogger<Initializer> _logger;
    private readonly DefaultProjectsConfiguration _defaultProjectsConfiguration;
    private readonly FdaClient _fdaClient;
    private readonly ProjectWork _projectWork;
    private readonly UserResolver _userResolver;
    private readonly BucketPrefixProvider _bucketPrefixProvider;
    private readonly LocalCache _localCache;
    private readonly OssBucket _bucket;
    private readonly ProjectService _projectService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public Initializer(ILogger<Initializer> logger, FdaClient fdaClient, IOptions<DefaultProjectsConfiguration> optionsAccessor,
                        ProjectWork projectWork, UserResolver userResolver, LocalCache localCache, ProjectService projectService,
                        BucketPrefixProvider bucketPrefixProvider)
    {
        _logger = logger;
        _fdaClient = fdaClient;
        _projectWork = projectWork;
        _userResolver = userResolver;
        _localCache = localCache;
        _projectService = projectService;
        _defaultProjectsConfiguration = optionsAccessor.Value;

        // bucket for anonymous user
        _bucket = _userResolver.AnonymousBucket;

        _bucketPrefixProvider = bucketPrefixProvider;
    }
    public async Task InitializeBundlesAsync()
    {
        using var scope = _logger.BeginScope("Init AppBundles and Activities");
        await _fdaClient.InitializeAsync();
    }
    public async Task InitializeAsync()
    {
        using var scope = _logger.BeginScope("Init");
        _logger.LogInformation("Initializing base data");

        // OSS bucket might fail to create, so repeat attempts
        var createBucketPolicy = Policy
            .Handle<ApiException>()
            .WaitAndRetryAsync(
                retryCount: 4,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan) => _logger.LogWarning("Cannot create OSS bucket. Repeating")
            );

        await Task.WhenAll(
                // create bundles and activities
                _fdaClient.InitializeAsync(),

                // create the bucket
                createBucketPolicy.ExecuteAsync(async () => await _bucket.CreateAsync())
            );

        _logger.LogInformation($"Bucket {_bucket.BucketKey} created");

        // publish default project files (specified by the appsettings.json)
        foreach (DefaultProjectConfiguration defaultProjectConfig in _defaultProjectsConfiguration.Projects)
        {
            var signedUrl = await _projectService.TransferProjectToOssAsync(_bucket, defaultProjectConfig);

            await _projectWork.AdoptAsync(defaultProjectConfig, signedUrl);
        }

        _logger.LogInformation("Added default projects.");
    }

    public async Task ClearAsync(bool deleteUserBuckets)
    {
        try
        {
            _logger.LogInformation($"Deleting anonymous user bucket {_bucket.BucketKey}");
            await _bucket.DeleteAsync();
            // We need to wait because the server needs some time to settle down. If we try to create the bucket again immediately we'll get a conflict error.
            await Task.Delay(4000);
        }
        catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
        {
            _logger.LogInformation($"Nothing to delete because bucket {_bucket.BucketKey} does not exist yet");
        }

        if (deleteUserBuckets)
        {
            _logger.LogInformation($"Deleting user buckets for registered users");
            // delete all user buckets
            var buckets = await _bucket.GetBucketsAsync();
            string userBucketPrefix = _bucketPrefixProvider.GetBucketPrefix();
            foreach (string bucket in buckets)
            {
                if (bucket.Contains(userBucketPrefix))
                {
                    _logger.LogInformation($"Deleting user bucket {bucket}");
                    await _bucket.DeleteBucketAsync(bucket);
                }
            }
        }

        // delete bundles and activities
        await _fdaClient.CleanUpAsync();

        // cleanup locally cached files but keep the directory as it is initialized at the start of the app and needed to run the web server
        Directory.EnumerateFiles(_localCache.LocalRootName).ToList().ForEach((string filePath) => File.Delete(filePath));
        Directory.EnumerateDirectories(_localCache.LocalRootName).ToList().ForEach((string dirPath) => Directory.Delete(dirPath, true));
    }
}
