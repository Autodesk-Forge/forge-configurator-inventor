using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    internal class Publisher
    {
        private readonly ForgeAppConfigBase _appConfig;
        private string _nickname;

        private readonly DesignAutomationClient _client;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(ForgeAppConfigBase appConfig, DesignAutomationClient client)
        {
            _appConfig = appConfig;
            _client = client;
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
            Console.WriteLine($"Created WI {status.Id}");
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                Console.Write(".");
                Thread.Sleep(2000);
                status = await _client.GetWorkitemStatusAsync(status.Id);
            }

            Console.WriteLine($"WI {status.Id} completed with {status.Status}");

            // dump report
            var client = new HttpClient();
            var report = await client.GetStringAsync(status.ReportUrl);
            Console.Write(report);
            Console.WriteLine();
        }

        protected async Task PostAppBundleAsync(string packagePathname)
        {
            if (!File.Exists(packagePathname))
                throw new Exception("App Bundle with package is not found.");

            var shortAppBundleId = $"{_appConfig.Bundle.Id}+{_appConfig.Label}";
            Console.WriteLine($"Posting app bundle '{shortAppBundleId}'.");

            // try to get already existing bundle
            var response = await _client.AppBundlesApi.GetAppBundleAsync(shortAppBundleId, throwOnError: false);
            if (response.HttpResponse.StatusCode == HttpStatusCode.NotFound) // create new bundle
            {
                await _client.CreateAppBundleAsync(_appConfig.Bundle, _appConfig.Label, packagePathname);
                Console.WriteLine("Created new app bundle.");
            }
            else // create new bundle version
            {
                var version = await _client.UpdateAppBundleAsync(_appConfig.Bundle, _appConfig.Label, packagePathname);
                Console.WriteLine($"Created version #{version} for '{shortAppBundleId}' app bundle.");
            }
        }

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

            // check if the activity exists already
            var response = await _client.ActivitiesApi.GetActivityAsync(await GetFullActivityId(), throwOnError: false);
            if (response.HttpResponse.StatusCode == HttpStatusCode.NotFound) // create activity
            {
                Console.WriteLine($"Creating activity '{_appConfig.ActivityId}'");
                await _client.CreateActivityAsync(activity, _appConfig.ActivityLabel);
                Console.WriteLine("Done");
            }
            else // add new activity version
            {
                Console.WriteLine("Found existing activity. Updating...");
                int version = await _client.UpdateActivityAsync(activity, _appConfig.ActivityLabel);
                Console.WriteLine($"Created version #{version} for '{_appConfig.ActivityId}' activity.");
            }
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
                Console.WriteLine($"Removing existing app bundle. Deleting {bundleId}...");
                await _client.AppBundlesApi.DeleteAppBundleAsync(bundleId);
            }
            else
            {
                Console.WriteLine($"The app bundle {bundleId} does not exist.");
            }

            //check activity exists already
            var activityResponse = await _client.ActivitiesApi.GetActivityAsync(await GetFullActivityId(), throwOnError: false);
            if (activityResponse.HttpResponse.StatusCode == HttpStatusCode.OK)
            {
                //remove existed activity
                Console.WriteLine($"Removing existing activity. Deleting {activityId}...");
                await _client.ActivitiesApi.DeleteActivityAsync(activityId);
            }
            else
            {
                Console.WriteLine($"The activity {activityId} does not exist.");
            }
        }
    }
}
