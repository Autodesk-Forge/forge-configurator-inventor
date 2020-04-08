using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication.Utilities
{
    internal class Publisher
    {
        private readonly ForgeAppConfigBase _appConfig;
        private readonly string _packagePathname;
        private string _nickname;

        internal DesignAutomationClient Client { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Publisher(IConfiguration configuration, ForgeAppConfigBase appConfig, string packagePathname)
        {
            _appConfig = appConfig;
            _packagePathname = packagePathname;
            Client = CreateDesignAutomationClient(configuration);
        }

        public async Task PostAppBundleAsync()
        {
            if (!File.Exists(_packagePathname))
                throw new Exception("App Bundle with package is not found.");

            var shortAppBundleId = $"{_appConfig.Bundle.Id}+{_appConfig.Label}";
            Console.WriteLine($"Posting app bundle '{shortAppBundleId}'.");

            // try to get already existing bundle
            var response = await Client.AppBundlesApi.GetAppBundleAsync(shortAppBundleId, throwOnError: false);
            if (response.HttpResponse.StatusCode == HttpStatusCode.NotFound) // create new bundle
            {
                await Client.CreateAppBundleAsync(_appConfig.Bundle, _appConfig.Label, _packagePathname);
                Console.WriteLine("Created new app bundle.");
            }
            else // create new bundle version
            {
                var version = await Client.UpdateAppBundleAsync(_appConfig.Bundle, _appConfig.Label, _packagePathname);
                Console.WriteLine($"Created version #{version} for '{shortAppBundleId}' app bundle.");
            }
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
            var status = await Client.CreateWorkItemAsync(wi);
            Console.WriteLine($"Created WI {status.Id}");
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                Console.Write(".");
                Thread.Sleep(2000);
                status = await Client.GetWorkitemStatusAsync(status.Id);
            }

            Console.WriteLine();
            Console.WriteLine($"WI {status.Id} completed with {status.Status}");
            Console.WriteLine();

            // dump report
            var client = new HttpClient();
            var report = await client.GetStringAsync(status.ReportUrl);
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(report);
            Console.ForegroundColor = oldColor;
            Console.WriteLine();
        }

        private async Task<string> GetFullActivityId()
        {
            string nickname = await GetNicknameAsync();
            return $"{nickname}.{_appConfig.ActivityId}+{_appConfig.ActivityLabel}";
        }

        public async Task<string> GetNicknameAsync()
        {
            if (_nickname == null)
            {
                _nickname = await Client.GetNicknameAsync("me");
            }

            return _nickname;
        }

        public async Task PublishActivityAsync()
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
            var response = await Client.ActivitiesApi.GetActivityAsync(await GetFullActivityId(), throwOnError: false);
            if (response.HttpResponse.StatusCode == HttpStatusCode.NotFound) // create activity
            {
                Console.WriteLine($"Creating activity '{_appConfig.ActivityId}'");
                await Client.CreateActivityAsync(activity, _appConfig.ActivityLabel);
                Console.WriteLine("Done");
            }
            else // add new activity version
            {
                Console.WriteLine("Found existing activity. Updating...");
                int version = await Client.UpdateActivityAsync(activity, _appConfig.ActivityLabel);
                Console.WriteLine($"Created version #{version} for '{_appConfig.ActivityId}' activity.");
            }
        }

        public async Task CleanExistingAppActivityAsync()
        {
            var bundleId = _appConfig.Bundle.Id;
            var activityId = _appConfig.ActivityId;
            var shortAppBundleId = $"{bundleId}+{_appConfig.Label}";


            //check app bundle exists already
            var appResponse = await Client.AppBundlesApi.GetAppBundleAsync(shortAppBundleId, throwOnError: false);
            if (appResponse.HttpResponse.StatusCode == HttpStatusCode.OK)
            {
                //remove existed app bundle 
                Console.WriteLine($"Removing existing app bundle. Deleting {bundleId}...");
                await Client.AppBundlesApi.DeleteAppBundleAsync(bundleId);
            }
            else
            {
                Console.WriteLine($"The app bundle {bundleId} does not exist.");
            }

            //check activity exists already
            var activityResponse = await Client.ActivitiesApi.GetActivityAsync(await GetFullActivityId(), throwOnError: false);
            if (activityResponse.HttpResponse.StatusCode == HttpStatusCode.OK)
            {
                //remove existed activity
                Console.WriteLine($"Removing existing activity. Deleting {activityId}...");
                await Client.ActivitiesApi.DeleteActivityAsync(activityId);
            }
            else
            {
                Console.WriteLine($"The activity {activityId} does not exist.");
            }
        }


        private static DesignAutomationClient CreateDesignAutomationClient(IConfiguration configuration)
        {
            var forgeService = CreateForgeService(configuration);

            var rsdkCfg = configuration.GetSection("DesignAutomation").Get<Configuration>();
            var options = (rsdkCfg == null) ? null : Options.Create(rsdkCfg);
            return new DesignAutomationClient(forgeService, options);
        }

        private static ForgeService CreateForgeService(IConfiguration configuration)
        {
            var forgeCfg = configuration.GetSection("Forge").Get<ForgeConfiguration>();
            var httpMessageHandler = new ForgeHandler(Options.Create(forgeCfg))
            {
                InnerHandler = new HttpClientHandler()
            };

            return new ForgeService(new HttpClient(httpMessageHandler));
        }
    }
}
