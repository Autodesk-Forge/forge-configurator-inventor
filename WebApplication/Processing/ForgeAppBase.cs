using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

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
        public Task Initialize(string packagePathname)
        {
            return Publisher.Initialize(packagePathname, this);
        }

        /// <summary>
        /// Remove app bundle and activity.
        /// </summary>
        /// <returns></returns>
        public Task CleanUp()
        {
            return Publisher.CleanUpAsync(this);
        }

        /// <summary>
        /// Run work items.
        /// </summary>
        /// <param name="args">Work item arguments.</param>
        protected Task<WorkItemStatus> Run(Dictionary<string, IArgument> args)
        {
            return Publisher.RunWorkItemAsync(args, this);
        }

        /// <summary>
        /// Process IPT or Zipped IAM file.
        /// </summary>
        public Task<WorkItemStatus> Process(AdoptionData projectData)
        {
            var args = projectData.IsAssembly ? ToIamArguments(projectData) : ToIptArguments(projectData);

            return Run(args);
        }

        /// <summary>
        /// Process IPT file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="outputUrl"></param>
        public Task<WorkItemStatus> ProcessIpt(string url, string outputUrl)
        {
            var args = ToIptArguments(url, outputUrl);
            return Run(args);
        }

        /// <summary>
        /// Process Zipped IAM file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pathInZip"></param>
        /// <param name="outputUrl"></param>
        public Task<WorkItemStatus> ProcessZippedIam(string url, string pathInZip, string outputUrl)
        {
            var args = ToIamArguments(url, pathInZip, outputUrl);
            return Run(args);
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
        /// Activity parameters.
        /// </summary>
        public abstract Dictionary<string, Parameter> ActivityParams { get; }

        /// <summary>
        /// Pick required output URL from the adoption data (which contains everything).
        /// </summary>
        protected abstract string OutputUrl(AdoptionData projectData);

        public Dictionary<string, IArgument> ToIptArguments(string url, string outputUrl)
        {
            return new Dictionary<string, IArgument>
            {
                {
                    InputParameterName,
                    new XrefTreeArgument { Url = url }
                },
                {
                    OutputParameterName,
                    new XrefTreeArgument { Verb = Verb.Put, Url = outputUrl }
                }
            };
        }

        public Dictionary<string, IArgument> ToIamArguments(string url, string pathInZip, string outputUrl)
        {
            return new Dictionary<string, IArgument>
            {
                {
                    InputParameterName,
                    new XrefTreeArgument { PathInZip = pathInZip, LocalName = "zippedIam.zip", Url = url }
                },
                {
                    OutputParameterName,
                    new XrefTreeArgument { Verb = Verb.Put, Url = outputUrl }
                }
            };
        }

        /// <summary>
        /// Name of the input parameter.
        /// </summary>
        public const string InputParameterName = "InventorDoc";

        /// <summary>
        /// Name of the output parameter.
        /// </summary>
        public string OutputParameterName => Id + "Output";

        public virtual Dictionary<string, IArgument> ToIptArguments(AdoptionData projectData)
        {
            return ToIptArguments(projectData.InputUrl, OutputUrl(projectData));
        }

        public virtual Dictionary<string, IArgument> ToIamArguments(AdoptionData projectData)
        {
            return ToIamArguments(projectData.InputUrl, projectData.TLA, OutputUrl(projectData));
        }
    }
}