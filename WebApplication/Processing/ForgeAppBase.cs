﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Abstract class for Forge App definition.
    /// Override the class and provide data which is specific for your forge app.
    /// </summary>
    public abstract class ForgeAppBase
    {
        public readonly string Engine = "Autodesk.Inventor+24"; // use version 24
        public readonly string Label = "alpha";

        public abstract string Id { get; }
        public abstract string Description { get; }

        /// <summary>
        /// Set to <c>false</c> for aggregated activity, which uses external bundles.
        /// </summary>
        public virtual bool HasBundle => true;

        public AppBundle Bundle
        {
            get
            {
                if (_appBundle == null)
                {
                    _appBundle = new AppBundle
                    {
                        Engine = Engine,
                        Id = Id,
                        Description = Description
                    };
                }
                return _appBundle;
            }
        }

        private AppBundle _appBundle;

        public string ActivityId => Id;
        public string ActivityLabel => Label;

        protected Publisher Publisher { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ForgeAppBase(Publisher publisher)
        {
            Publisher = publisher;
        }

        /// <summary>
        /// Initialize app bundle and activity.
        /// </summary>
        /// <param name="packagePathname">Pathname to the package.</param>
        public Task InitializeAsync(string packagePathname)
        {
            return Publisher.InitializeAsync(packagePathname, this);
        }

        /// <summary>
        /// Remove app bundle and activity.
        /// </summary>
        /// <returns></returns>
        public Task CleanUpAsync()
        {
            return Publisher.CleanUpAsync(this);
        }

        /// <summary>
        /// Run work item and wait for the completion.
        /// </summary>
        /// <param name="args">Work item arguments.</param>
        protected async Task<bool> RunAsync(Dictionary<string, IArgument> args)
        {
            WorkItemStatus status = await Publisher.RunWorkItemAsync(args, this);
            return status.Status == Status.Success;
        }

        /// <summary>
        /// Process IPT or Zipped IAM file.
        /// </summary>
        public Task<bool> ProcessAsync(AdoptionData projectData)
        {
            var args = ToWorkItemArgs(projectData);

            return RunAsync(args);
        }

        public virtual Dictionary<string, IArgument> ToWorkItemArgs(AdoptionData projectData)
        {
            var args = new Dictionary<string, IArgument>();
            AddInputArgs(args, projectData);

            if (HasOutput)
            {
                AddOutputArgs(args, projectData);
            }

            return args;
        }

        protected virtual void AddOutputArgs(IDictionary<string, IArgument> args, AdoptionData projectData)
        {
            args.Add(OutputParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = OutputUrl(projectData) });
        }

        protected virtual void AddInputArgs(IDictionary<string, IArgument> args, AdoptionData projectData)
        {
            if (projectData.IsAssembly)
                args.Add(InputParameterName, new XrefTreeArgument { PathInZip = projectData.TLA, LocalName = "zippedIam.zip", Url = projectData.InputUrl });
            else
                args.Add(InputParameterName, new XrefTreeArgument { Url = projectData.InputUrl });
        }

        /// <summary>
        /// Get list of bundles referenced by the activity.
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public virtual List<string> GetBundles(string nickname)
        {
            return new List<string> { $"{nickname}.{Id}+{Label}" };
        }

        /// <summary>
        /// Command line for activity.
        /// </summary>
        public virtual List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{InputParameterName}].path)"
            };

        /// <summary>
        /// Pick required output URL from the adoption data (which contains everything).
        /// </summary>
        protected virtual string OutputUrl(AdoptionData projectData) => throw new NotImplementedException();

        /// <summary>
        /// Name of the input parameter.
        /// </summary>
        public const string InputParameterName = "InventorDoc";

        /// <summary>
        /// Name of the output parameter.
        /// </summary>
        public string OutputParameterName => Id + "Output";

        /// <summary>
        /// Activity parameters.
        /// </summary>
        public virtual Dictionary<string, Parameter> ActivityParams
        {
            get
            {
                var activityParams = new Dictionary<string, Parameter>
                                        {
                                            {
                                                InputParameterName,
                                                new Parameter
                                                {
                                                    Verb = Verb.Get,
                                                    Description = "IPT or IAM (in ZIP) file to process"
                                                }
                                            }
                                        };

                if (HasOutput)
                {
                    activityParams.Add(OutputParameterName, new Parameter { Verb = Verb.Put, LocalName = OutputName, Zip = IsOutputZip });
                }

                return activityParams;
            }
        }

        /// <summary>
        /// Filename of output (processing result of Inventor add-on).
        /// </summary>
        protected virtual string OutputName => throw new NotImplementedException();

        /// <summary>
        /// If output should be compressed as ZIP.
        /// </summary>
        protected virtual bool IsOutputZip => false;

        protected virtual bool HasOutput => true;
    }
}