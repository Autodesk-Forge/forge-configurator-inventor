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
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    [Flags]
    public enum ForgeRegistration
    {
        AppBundle = 0x1,
        Activity  = 0x2,

        /// <summary>
        /// Register both app bundle and activity.
        /// </summary>
        All = AppBundle | Activity
    }

    /// <summary>
    /// Abstract class for Forge App definition.
    /// Override the class and provide data which is specific for your forge app.
    /// </summary>
    public abstract class ForgeAppBase
    {
        public virtual string Engine { protected set; get; } = "Autodesk.Inventor+2021";
        public readonly string Label = Environment.GetEnvironmentVariable("ProductionEnv") ?? "alpha";
        public abstract string Id { get; }
        public abstract string Description { get; }

        /// <summary>
        /// What to register at Forge.
        /// NOTE: by default it registers AppBundle only!
        /// </summary>
        protected internal virtual ForgeRegistration Registration { get; } = ForgeRegistration.AppBundle;

        /// <summary>
        /// If processing needs to register app bundle.
        /// </summary>
        public bool HasBundle => (Registration & ForgeRegistration.AppBundle) != 0;

        /// <summary>
        /// If processing needs to register app bundle.
        /// </summary>
        public bool HasActivity => (Registration & ForgeRegistration.Activity) != 0;

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
        public async Task InitializeAsync(string packagePathname)
        {
            await Publisher.InitializeAsync(packagePathname, this);
        }

        /// <summary>
        /// Remove app bundle and activity.
        /// </summary>
        /// <returns></returns>
        public async Task CleanUpAsync()
        {
            await Publisher.CleanUpAsync(this);
        }

        /// <summary>
        /// Run work item and wait for the completion.
        /// </summary>
        /// <param name="args">Work item arguments.</param>
        protected async Task<ProcessingResult> RunAsync(Dictionary<string, IArgument> args)
        {
            WorkItemStatus status = await Publisher.RunWorkItemAsync(args, this);
            return new ProcessingResult(status.Stats)
                    {
                        Success = (status.Status == Status.Success),
                        ReportUrl = status.ReportUrl
                    };
        }

        /// <summary>
        /// Process IPT or Zipped IAM file.
        /// </summary>
        public Task<ProcessingResult> ProcessAsync(ProcessingArgs data)
        {
            var args = ToWorkItemArgs(data);

            return RunAsync(args);
        }

        /// <summary>
        /// Add input and output arguments for the work item.
        /// </summary>
        public virtual Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var args = new Dictionary<string, IArgument>();
            AddInputArgs(args, data);

            if (HasOutput)
            {
                args.Add(OutputParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = OutputUrl(data), Optional = IsOutputOptional });
            }

            return args;
        }

        /// <summary>
        /// Add input arguments for work item.
        /// </summary>
        protected virtual void AddInputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            if (data.IsAssembly)
                args.Add(InputDocParameterName, new XrefTreeArgument { PathInZip = data.TLA, LocalName = FolderToBeZippedName, Url = data.InputDocUrl });
            else
                args.Add(InputDocParameterName, new XrefTreeArgument { Url = data.InputDocUrl, LocalName = IptName });
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
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i \"$(args[{InputDocParameterName}].path)\""
            };

        /// <summary>
        /// Pick required output URL from the processing data (which contains everything).
        /// </summary>
        protected virtual string OutputUrl(ProcessingArgs data) => throw new NotImplementedException();

        /// <summary>
        /// Parameter name for input document.
        /// </summary>
        public const string InputDocParameterName = "InventorDoc";

        /// <summary>
        /// Where zip stored at DA servers.
        /// </summary>
        /// <remarks>NOTE: arg name is misleading, this actually is a dirname, where the zip is extracted.</remarks>
        protected const string FolderToBeZippedName = "unzippedIam";

        protected const string IptName = "part.ipt";

        /// <summary>
        /// Name of the output parameter.
        /// </summary>
        public string OutputParameterName => Id + "Output";

        /// <summary>
        /// Activity parameters.
        /// </summary>
        public virtual Dictionary<string, Parameter> GetActivityParams()
        {
            var activityParams = new Dictionary<string, Parameter>
            {
                {
                    InputDocParameterName,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "IPT or IAM (in ZIP) file to process"
                    }
                }
            };

            if (HasOutput)
            {
                activityParams.Add(OutputParameterName,
                    new Parameter {Verb = Verb.Put, LocalName = OutputName, Zip = IsOutputZip});
            }

            return activityParams;
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
        protected virtual bool IsOutputOptional => false;
    }
}