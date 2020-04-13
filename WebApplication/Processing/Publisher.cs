using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
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
        private readonly DesignAutomationClient _client;
        private readonly ILogger<Publisher> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(DesignAutomationClient client, ILogger<Publisher> logger, ResourceProvider resourceProvider)
        {
            _client = client;
            _logger = logger;
            _resourceProvider = resourceProvider;
        }

        public async Task RunWorkItemAsync(Dictionary<string, IArgument> workItemArgs, ForgeAppConfigBase config)
        {
            // create work item
            var wi = new WorkItem
            {
                ActivityId = await GetFullActivityId(config),
                Arguments = workItemArgs
            };

            // run WI and wait for completion
            var status = await _client.CreateWorkItemAsync(wi);
            Trace($"Created WI {status.Id}");
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                //Console.Write(".");
                Thread.Sleep(2000);
                status = await _client.GetWorkitemStatusAsync(status.Id);
            }

            Trace($"WI {status.Id} completed with {status.Status}");
            Trace($"{status.ReportUrl}");
        }

        protected async Task PostAppBundleAsync(string packagePathname, ForgeAppConfigBase config)
        {
            if (!File.Exists(packagePathname))
                throw new Exception("App Bundle with package is not found.");

            Trace($"Posting app bundle '{config.Bundle}'.");
            await _client.CreateAppBundleAsync(config.Bundle, config.Label, packagePathname);
        }


        /// <summary>
        /// Create new activity.
        /// Throws an exception if the activity exists already.
        /// </summary>
        protected async Task PublishActivityAsync(ForgeAppConfigBase config)
        {
            // prepare activity definition
            var nickname = await _resourceProvider.GetNicknameAsync();

            var activity = new Activity
            {
                Appbundles = new List<string> { $"{nickname}.{config.Id}+{config.Label}" },
                Id = config.ActivityId,
                Engine = config.Engine,
                Description = config.Description,
                CommandLine = config.ActivityCommandLine,
                Parameters = config.ActivityParams
            };

            Trace($"Creating activity '{config.ActivityId}'");
            await _client.CreateActivityAsync(activity, config.ActivityLabel);
        }

        /// <summary>
        /// Create app bundle and activity.
        /// </summary>
        /// <param name="packagePathname">Pathname to ZIP with app bundle.</param>
        /// <param name="config"></param>
        public async Task Initialize(string packagePathname, ForgeAppConfigBase config)
        {
            await PostAppBundleAsync(packagePathname, config);
            await PublishActivityAsync(config);
        }

        /// <summary>
        /// Delete app bundle and activity.
        /// </summary>
        public async Task CleanUpAsync(ForgeAppConfigBase config)
        {
            var bundleId = config.Bundle.Id;
            var shortBundleId = $"{config.Bundle.Id}+{config.Label}";

            //check app bundle exists already
            var appResponse = await _client.AppBundlesApi.GetAppBundleAsync(shortBundleId, throwOnError: false);
            if (appResponse.HttpResponse.StatusCode == HttpStatusCode.OK)
            {
                //remove existed app bundle 
                Trace($"Removing existing app bundle. Deleting {bundleId}...");
                await _client.AppBundlesApi.DeleteAppBundleAsync(bundleId);
            }
            else
            {
                Trace($"The app bundle {bundleId} does not exist.");
            }

            //check activity exists already
            var activityId = config.ActivityId;
            var fullActivityId = await GetFullActivityId(config);
            var activityResponse = await _client.ActivitiesApi.GetActivityAsync(fullActivityId, throwOnError: false);
            if (activityResponse.HttpResponse.StatusCode == HttpStatusCode.OK)
            {
                //remove existed activity
                Trace($"Removing existing activity. Deleting {activityId}...");
                await _client.ActivitiesApi.DeleteActivityAsync(activityId);
            }
            else
            {
                Trace($"The activity {activityId} does not exist.");
            }
        }

        private async Task<string> GetFullActivityId(ForgeAppConfigBase config)
        {
            var nickname = await _resourceProvider.GetNicknameAsync();
            return $"{nickname}.{config.ActivityId}+{config.ActivityLabel}";
        }

        private void Trace(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
