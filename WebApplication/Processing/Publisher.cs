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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Utilities;
using Activity = Autodesk.Forge.DesignAutomation.Model.Activity;

namespace WebApplication.Processing
{
    public class Publisher
    {
        private readonly ResourceProvider _resourceProvider;
        private readonly IPostProcessing _postProcessing;
        private readonly DesignAutomationClient _client;
        private readonly ILogger<Publisher> _logger;
        private readonly string _callbackUrlBase;

        /// <summary>
        /// How a work item completion check should be done.
        /// </summary>
        public CompletionCheck CompletionCheck { get; set; } // TECHDEBT: setter should not be public, but it's a easiest way to allow use polling for initialization phase

        /// <summary>
        /// Tracker of WI jobs.
        /// Used for 'callback' mode only.
        /// </summary>
        public ConcurrentDictionary<string, TaskCompletionSource<WorkItemStatus>> Tracker { get; } =
            new ConcurrentDictionary<string, TaskCompletionSource<WorkItemStatus>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(DesignAutomationClient client, ILogger<Publisher> logger, ResourceProvider resourceProvider, 
            IPostProcessing postProcessing, IOptions<PublisherConfiguration> publisherConfiguration)
        {
            _client = client;
            _logger = logger;
            _resourceProvider = resourceProvider;
            _postProcessing = postProcessing;

            _callbackUrlBase = publisherConfiguration.Value.CallbackUrlBase;
            CompletionCheck = publisherConfiguration.Value.CompletionCheck;
        }

        public async Task<WorkItemStatus> RunWorkItemAsync(Dictionary<string, IArgument> workItemArgs, ForgeAppBase config)
        {
            // create work item
            var wi = new WorkItem
            {
                ActivityId = await GetFullActivityId(config),
                Arguments = workItemArgs
            };

            // run WI and wait for completion
            var sw = Stopwatch.StartNew();
            WorkItemStatus status = await LaunchAndWait(wi);

            _logger.LogInformation($"WI {status.Id} completed with {status.Status} in {sw.ElapsedMilliseconds} ms");
            _logger.LogInformation($"{status.ReportUrl}");

            await _postProcessing.HandleStatus(status);
            return status;
        }

        /// <summary>
        /// Run the work item and wait for results.
        /// </summary>
        private async Task<WorkItemStatus> LaunchAndWait(WorkItem wi)
        {
            //use polling if not configured otherwise
            return CompletionCheck switch
            {
                CompletionCheck.Callback => await RunWithCallback(wi),
                _ => await RunWithPolling(wi)
            };
        }

        private async Task<WorkItemStatus> RunWithPolling(WorkItem wi)
        {
            WorkItemStatus status = await _client.CreateWorkItemAsync(wi);
            _logger.LogInformation($"Created WI {status.Id} (polling mode)");
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                await Task.Delay(2000);
                status = await _client.GetWorkitemStatusAsync(status.Id);
            }

            return status;
        }

        private async Task<WorkItemStatus> RunWithCallback(WorkItem wi)
        {
            // register tracking key for callback task
            string trackingKey = Guid.NewGuid().ToString("N");
            var completionSource = Tracker.GetOrAdd(trackingKey, new TaskCompletionSource<WorkItemStatus>());

            // build callback URL to be poked from FDA server on WI completion
            string callbackUrl = _callbackUrlBase + trackingKey;
            var callbackOnComplete = new XrefTreeArgument { Verb = Verb.Post, Url = callbackUrl };
            wi.Arguments.Add("onComplete", callbackOnComplete);

            // post work item
            WorkItemStatus status = await _client.CreateWorkItemAsync(wi);
            _logger.LogInformation($"Created WI {status.Id} with tracker ID {trackingKey}");

            // wait for completion
            status = await completionSource.Task;

            _logger.LogInformation($"Completing WI {status.Id} with tracker ID {trackingKey}");

            return status;
        }

        private async Task PostAppBundleAsync(string packagePathname, ForgeAppBase config)
        {
            if (!File.Exists(packagePathname))
                throw new Exception($"App Bundle package is not found ({packagePathname})");

            try {
                // checking existence of the AppBundle
                await _client.GetAppBundleVersionsAsync(config.Bundle.Id);
                Alias oldVersion = null;
                try {
                    // getting version of AppBundle for the particular Alias (label)
                    oldVersion = await _client.GetAppBundleAliasAsync(config.Bundle.Id, config.Label);
                } catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404")) {};
                _logger.LogInformation($"Creating new app bundle '{config.Bundle.Id}' version.");
                await _client.UpdateAppBundleAsync(config.Bundle, config.Label, packagePathname);
                if (oldVersion != null)
                    await _client.DeleteAppBundleVersionAsync(config.Bundle.Id, oldVersion.Version);
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404"))
            {
                _logger.LogInformation($"Creating app bundle '{config.Bundle.Id}'.");
                await _client.CreateAppBundleAsync(config.Bundle, config.Label, packagePathname);
            }
        }


        /// <summary>
        /// Create new activity.
        /// Throws an exception if the activity exists already.
        /// </summary>
        private async Task PublishActivityAsync(ForgeAppBase config)
        {
            // prepare activity definition
            var nickname = await _resourceProvider.Nickname;

            var activity = new Activity
            {
                Appbundles = config.GetBundles(nickname),
                Id = config.ActivityId,
                Engine = config.Engine,
                Description = config.Description,
                CommandLine = config.ActivityCommandLine,
                Parameters = config.GetActivityParams()
            };

            try {
                // checking existence of Activity
                await _client.GetActivityVersionsAsync(config.ActivityId);
                Alias oldVersion = null;
                try {
                    // getting version of Activity for the particular Alias (label)
                    oldVersion = await _client.GetActivityAliasAsync(config.ActivityId, config.ActivityLabel);
                } catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404")) {};
                _logger.LogInformation($"Creating new activity '{config.ActivityId}' version.");
                await _client.UpdateActivityAsync(activity, config.ActivityLabel);
                if (oldVersion != null)
                    await _client.DeleteActivityVersionAsync(config.ActivityId, oldVersion.Version);
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404"))
            {
                _logger.LogInformation($"Creating activity '{config.ActivityId}'");
                await _client.CreateActivityAsync(activity, config.ActivityLabel);
            }
        }

        /// <summary>
        /// Create app bundle and activity.
        /// </summary>
        /// <param name="packagePathname">Pathname to ZIP with app bundle.</param>
        /// <param name="config"></param>
        public async Task InitializeAsync(string packagePathname, ForgeAppBase config)
        {
            if (config.HasBundle)
            {
                await PostAppBundleAsync(packagePathname, config);
            }

            if (config.HasActivity)
            {
                await PublishActivityAsync(config);
            }
        }

        /// <summary>
        /// Delete app bundle and activity.
        /// </summary>
        public async Task CleanUpAsync(ForgeAppBase config)
        {
            //remove activity
            if (config.HasActivity)
            {
                _logger.LogInformation($"Removing '{config.ActivityId}' activity.");
                await _client.ActivitiesApi.DeleteActivityAsync(config.ActivityId, null, null, false);
            }

            if (config.HasBundle)
            {
                //remove existed app bundle 
                _logger.LogInformation($"Removing '{config.Bundle.Id}' app bundle.");
                await _client.AppBundlesApi.DeleteAppBundleAsync(config.Bundle.Id, throwOnError: false);
            }
        }

        private async Task<string> GetFullActivityId(ForgeAppBase config)
        {
            var nickname = await _resourceProvider.Nickname;
            return $"{nickname}.{config.ActivityId}+{config.ActivityLabel}";
        }
    }
}
