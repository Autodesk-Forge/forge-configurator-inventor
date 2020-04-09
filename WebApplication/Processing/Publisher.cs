using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Logging;

namespace WebApplication.Processing
{
    internal class Publisher
    {
        private readonly ForgeAppConfigBase _appConfig;
        private string _nickname;

        private readonly DesignAutomationClient _client;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(ForgeAppConfigBase appConfig, DesignAutomationClient client, ILogger logger)
        {
            _appConfig = appConfig;
            _client = client;
            _logger = logger;
        }

        public async Task Initialize(string packagePathname)
        {
            await PostAppBundleAsync(packagePathname);
            await PublishActivityAsync();
        }

        public async Task RunWorkItemAsync()
        {
            // create work item
            var wi = new WorkItem
            {
                ActivityId = await GetFullActivityId(),
                Arguments = _appConfig.WorkItemArgs
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

            // dump report
            var client = new HttpClient();
            var report = await client.GetStringAsync(status.ReportUrl);
            Console.Write(report);
        }

        protected async Task PostAppBundleAsync(string packagePathname)
        {
            if (!File.Exists(packagePathname))
                throw new Exception("App Bundle with package is not found.");

            var shortAppBundleId = $"{_appConfig.Bundle.Id}+{_appConfig.Label}";
            Trace($"Posting app bundle '{shortAppBundleId}'.");

            await _client.CreateAppBundleAsync(_appConfig.Bundle, _appConfig.Label, packagePathname);
            Trace("Created new app bundle.");
        }

        /// <summary>
        /// Create new activity.
        /// Throws an exception if the activity exists already.
        /// </summary>
        /// <returns></returns>
        protected async Task PublishActivityAsync()
        {
            var nickname = await GetNicknameAsync();

            // prepare activity definition
            var activity = new Activity
            {
                Appbundles = new List<string> { $"{nickname}.{_appConfig.Id}+{_appConfig.Label}" },
                Id = _appConfig.ActivityId,
                Engine = _appConfig.Engine,
                Description = _appConfig.Description,
                CommandLine = _appConfig.ActivityCommandLine,
                Parameters = _appConfig.ActivityParams
            };


            Trace($"Creating activity '{_appConfig.ActivityId}'");
            await _client.CreateActivityAsync(activity, _appConfig.ActivityLabel);
        }

        private async Task<string> GetFullActivityId()
        {
            string nickname = await GetNicknameAsync();
            return $"{nickname}.{_appConfig.ActivityId}+{_appConfig.ActivityLabel}";
        }

        private async Task<string> GetNicknameAsync()
        {
            if (_nickname == null)
            {
                _nickname = await _client.GetNicknameAsync("me");
            }

            return _nickname;
        }

        /// <summary>
        /// Delete app bundle and activity.
        /// </summary>
        public async Task CleanUpAsync()
        {
            var bundleId = _appConfig.Bundle.Id;
            var activityId = _appConfig.ActivityId;
            var shortAppBundleId = $"{bundleId}+{_appConfig.Label}";

            //check app bundle exists already
            var appResponse = await _client.AppBundlesApi.GetAppBundleAsync(shortAppBundleId, throwOnError: false);
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
            var activityResponse = await _client.ActivitiesApi.GetActivityAsync(await GetFullActivityId(), throwOnError: false);
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
