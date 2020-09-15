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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    public class Publisher
    {
        private readonly ResourceProvider _resourceProvider;
        private readonly IPostProcessing _postProcessing;
        private readonly DesignAutomationClient _client;
        private readonly ILogger<Publisher> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(DesignAutomationClient client, ILogger<Publisher> logger, ResourceProvider resourceProvider, IPostProcessing postProcessing)
        {
            _client = client;
            _logger = logger;
            _resourceProvider = resourceProvider;
            _postProcessing = postProcessing;
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
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            WorkItemStatus status = await _client.CreateWorkItemAsync(wi);
            Trace($"Created WI {status.Id}");
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                await Task.Delay(2000);
                status = await _client.GetWorkitemStatusAsync(status.Id);
            }

            Trace($"WI {status.Id} completed with {status.Status} in {sw.ElapsedMilliseconds} ms");
            Trace($"{status.ReportUrl}");

            await _postProcessing.HandleStatus(status);
            return status;
        }

        private async Task PostAppBundleAsync(string packagePathname, ForgeAppBase config)
        {
            if (!File.Exists(packagePathname))
                throw new Exception($"App Bundle with package is not found ({packagePathname}).");

            try {
                // checking existence of the AppBundle
                await _client.GetAppBundleVersionsAsync(config.Bundle.Id);
                Alias oldVersion = null;
                try {
                    // getting version of AppBundle for the particular Alias (label)
                    oldVersion = await _client.GetAppBundleAliasAsync(config.Bundle.Id, config.Label);
                } catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404")) {};
                Trace($"Creating new app bundle '{config.Bundle.Id}' version.");
                await _client.UpdateAppBundleAsync(config.Bundle, config.Label, packagePathname);
                if (oldVersion != null)
                    await _client.DeleteAppBundleVersionAsync(config.Bundle.Id, oldVersion.Version);
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404"))
            {
                Trace($"Creating app bundle '{config.Bundle.Id}'.");
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
                Trace($"Creating new activity '{config.ActivityId}' version.");
                await _client.UpdateActivityAsync(activity, config.ActivityLabel);
                if (oldVersion != null)
                    await _client.DeleteActivityVersionAsync(config.ActivityId, oldVersion.Version);
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("404"))
            {
                Trace($"Creating activity '{config.ActivityId}'");
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
                Trace($"Removing '{config.ActivityId}' activity.");
                await _client.ActivitiesApi.DeleteActivityAsync(config.ActivityId, null, null, false);
            }

            if (config.HasBundle)
            {
                //remove existed app bundle 
                Trace($"Removing '{config.Bundle.Id}' app bundle.");
                await _client.AppBundlesApi.DeleteAppBundleAsync(config.Bundle.Id, throwOnError: false);
            }
        }

        private async Task<string> GetFullActivityId(ForgeAppBase config)
        {
            var nickname = await _resourceProvider.Nickname;
            return $"{nickname}.{config.ActivityId}+{config.ActivityLabel}";
        }

        private void Trace(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
