using System;
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
        public virtual string Engine { protected set; get; } = "Autodesk.Inventor+2021"; // use version 24
        public readonly string Label = Environment.GetEnvironmentVariable("ProductionEnv") ?? "alpha";
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
            return new ProcessingResult
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

        public virtual Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var args = new Dictionary<string, IArgument>();
            AddInputArgs(args, data);

            if (HasOutput)
            {
                AddOutputArgs(args, data);
            }

            return args;
        }

        protected virtual void AddOutputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            args.Add(OutputParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = OutputUrl(data) });
        }

        protected virtual void AddInputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            if (data.IsAssembly)
                args.Add(InputDocParameterName, new XrefTreeArgument { PathInZip = data.TLA, LocalName = ZipName, Url = data.InputDocUrl });
            else
                args.Add(InputDocParameterName, new XrefTreeArgument { Url = data.InputDocUrl });
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
        protected const string ZipName = "zippedIam.zip";

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
    }
}