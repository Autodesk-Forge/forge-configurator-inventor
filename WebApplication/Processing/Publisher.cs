using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;

namespace WebApplication.Processing
{
    public class Publisher
    {
        private readonly ForgeAppConfigBase _appConfig;
        private readonly string _nickname;

        private readonly DesignAutomationClient _client;
        private readonly ILogger _logger;

        private string FullActivityId => $"{_nickname}.{_appConfig.ActivityId}+{_appConfig.ActivityLabel}";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(ForgeAppConfigBase appConfig, DesignAutomationClient client, ILogger logger, string nickname)
        {
            _appConfig = appConfig;
            _client = client;
            _logger = logger;
            _nickname = nickname;
        }

        public async Task RunWorkItemAsync(Dictionary<string, IArgument> workItemArgs)
        {
            // create work item
            var wi = new WorkItem
            {
                ActivityId = FullActivityId,
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

        protected async Task PostAppBundleAsync(string packagePathname)
        {
            if (!File.Exists(packagePathname))
                throw new Exception("App Bundle with package is not found.");

            Trace($"Posting app bundle '{_appConfig.Bundle}'.");
            await _client.CreateAppBundleAsync(_appConfig.Bundle, _appConfig.Label, packagePathname);
        }


        /// <summary>
        /// Create new activity.
        /// Throws an exception if the activity exists already.
        /// </summary>
        /// <returns></returns>
        protected async Task PublishActivityAsync()
        {
            // prepare activity definition
            var activity = new Activity
            {
                Appbundles = new List<string> { $"{_nickname}.{_appConfig.Id}+{_appConfig.Label}" },
                Id = _appConfig.ActivityId,
                Engine = _appConfig.Engine,
                Description = _appConfig.Description,
                CommandLine = _appConfig.ActivityCommandLine,
                Parameters = _appConfig.ActivityParams
            };

            Trace($"Creating activity '{_appConfig.ActivityId}'");
            await _client.CreateActivityAsync(activity, _appConfig.ActivityLabel);
        }

        /// <summary>
        /// Create app bundle and activity.
        /// </summary>
        /// <param name="packagePathname">Pathname to ZIP with app bundle.</param>
        public async Task Initialize(string packagePathname)
        {
            await PostAppBundleAsync(packagePathname);
            await PublishActivityAsync();
        }

        /// <summary>
        /// Delete app bundle and activity.
        /// </summary>
        public async Task CleanUpAsync()
        {
            var bundleId = _appConfig.Bundle.Id;
            var shortBundleId = $"{_appConfig.Bundle.Id}+{_appConfig.Label}";

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
            var activityId = _appConfig.ActivityId;
            var activityResponse = await _client.ActivitiesApi.GetActivityAsync(FullActivityId, throwOnError: false);
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

        private void Trace(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
