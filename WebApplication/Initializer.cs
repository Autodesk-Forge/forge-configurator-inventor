using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.Utilities;

namespace IoConfigDemo
{
    internal class CreateSvfDefinition : ForgeAppConfigBase
    {
        public override int EngineVersion => 23;
        public override string Description => "Generate SVF from Inventor document";
        public override string Id => "CreateSvfForInventor";
        public override string Label => "alpha";

        internal static class Parameters
        {
            public static string InventorDoc = nameof(InventorDoc);
            public static string OutputIpt = nameof(OutputIpt);
        }

        /// <summary>
        /// Get command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{Parameters.InventorDoc}].path)"
            };

        /// <summary>
        /// Get activity parameters.
        /// </summary>
        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
                    {
                        {
                            Parameters.InventorDoc,
                            new Parameter
                            {
                                Verb = Verb.Get,
                                Description = "IPT file to process"
                            }
                        },
                        {
                            Parameters.OutputIpt,
                            new Parameter
                            {
                                Verb = Verb.Put,
                                LocalName = "result.ipt",
                                Description = "Resulting IPT",
                                Ondemand = false,
                                Required = false
                            }
                        }
                    };

        /// <summary>
        /// Get arguments for workitem.
        /// </summary>
        public override Dictionary<string, IArgument> WorkItemArgs =>
            new Dictionary<string, IArgument>
                    {
                        {
                            Parameters.InventorDoc,
                            new XrefTreeArgument
                            {
                                Url = "!!! CHANGE ME !!!"
                            }
                        },
                        {
                            Parameters.OutputIpt,
                            new XrefTreeArgument
                            {
                                Verb = Verb.Put,
                                Url = "!!! CHANGE ME !!!"
                            }
                        }
                    };
    }

    public class Initializer
    {
        private readonly IForge _forge;
        private readonly BucketNameProvider _bucketNameProvider;
        private readonly ILogger<Initializer> _logger;
        public Initializer(IForge forge, BucketNameProvider bucketNameProvider, ILogger<Initializer> logger)
        {
            _forge = forge;
            _bucketNameProvider = bucketNameProvider;
            _logger = logger;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Initializing base data");
            await _forge.CreateBucket(_bucketNameProvider.BucketName);
            _logger.LogInformation($"Bucket {_bucketNameProvider.BucketName} created");
            
            await Task.WhenAll(
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, "Project1.zip"),
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, "Project2.zip"),
                _forge.CreateEmptyObject(_bucketNameProvider.BucketName, "Project3.zip")
            );
            _logger.LogInformation("Added empty projects.");

            // create bundles and activities
        }

        public async Task Clear()
        {
            try
            {
                await _forge.DeleteBucket(_bucketNameProvider.BucketName);
                // We need to wait because server needs some time to settle it down. If we would go and create bucket immediately again we would receive conflict.
                await Task.Delay(4000);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                _logger.LogInformation($"Nothing to delete because bucket {_bucketNameProvider.BucketName} does not exists yet");
            }

            // delete bundles and activities
        }
    }
}