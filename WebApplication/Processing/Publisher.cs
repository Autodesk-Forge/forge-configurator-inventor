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
            DateTime start = DateTime.Now;
            WorkItemStatus status = await _client.CreateWorkItemAsync(wi);
            Trace($"Created WI {status.Id}");
            while (status.Status == Status.Pending || status.Status == Status.Inprogress)
            {
                await Task.Delay(2000);
                status = await _client.GetWorkitemStatusAsync(status.Id);
            }

            var seconds = (DateTime.Now - start).TotalSeconds;
            Trace($"WI {status.Id} completed with {status.Status} in {seconds} seconds");
            Trace($"{status.ReportUrl}");

            await _postProcessing.HandleStatus(status);
            return status;
        }

        private async Task PostAppBundleAsync(string packagePathname, ForgeAppBase config)
        {
            if (! config.HasBundle) return;

            if (!File.Exists(packagePathname))
                throw new Exception($"App Bundle with package is not found ({packagePathname}).");

            Trace($"Creating app bundle '{config.Bundle.Id}'.");
            await _client.CreateAppBundleAsync(config.Bundle, config.Label, packagePathname);
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
        public async Task InitializeAsync(string packagePathname, ForgeAppBase config)
        {
            await PostAppBundleAsync(packagePathname, config);
            await PublishActivityAsync(config);
        }

        /// <summary>
        /// Delete app bundle and activity.
        /// </summary>
        public async Task CleanUpAsync(ForgeAppBase config)
        {
            //remove activity
            Trace($"Removing '{config.ActivityId}' activity.");
            await _client.ActivitiesApi.DeleteActivityAsync(config.ActivityId, null, null, false);

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
