using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    /// <summary>
    /// Takes single input (IPT or zipped IAM) and generates single output.
    /// </summary>
    public abstract class SimpleDocProcessing : ForgeAppBase
    {
        public override string OutputParameterName => Id + "Output";
        public override bool HasBundle => true;

        protected SimpleDocProcessing(Publisher publisher) : base(publisher) {}

        /// <summary>
        /// Pick required output URL from the adoption data (which contains everything).
        /// </summary>
        protected abstract string OutputUrl(AdoptionData projectData);

        public override Dictionary<string, IArgument> ToIptArguments(AdoptionData projectData)
        {
            return ToIptArguments(projectData.InputUrl, OutputUrl(projectData));
        }

        public override Dictionary<string, IArgument> ToIamArguments(AdoptionData projectData)
        {
            return ToIamArguments(projectData.InputUrl, projectData.TLA, OutputUrl(projectData));
        }

        /// <summary>
        /// Get command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{InputParameterName}].path)"
            };
    }
}