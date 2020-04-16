using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class CreateSVF : ForgeAppBase
    {
        public override int EngineVersion => 24;

        public override string Id => nameof(CreateSVF);
        public override string Label => "alpha";
        public override string Description => "Generate SVF from Inventor document";

        public override string OutputParameterName => "OutputZip";

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
                        Description = "IPT or IAM (in ZIP) file to process"
                    }
                },
                {
                    OutputParameterName, 
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "SvfOutput",
                        Description = "Resulting files with SVF",
                        //Ondemand = false,
                        //Required = false,
                        Zip = true
                    }
                }
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSVF(Publisher publisher) : base(publisher) {}

        public override Dictionary<string, IArgument> ToIptArguments(AdoptionData projectData)
        {
            return ToIptArguments(projectData.InputUrl, projectData.SvfUrl);
        }

        public override Dictionary<string, IArgument> ToIamArguments(AdoptionData projectData)
        {
            return ToIamArguments(projectData.InputUrl, projectData.TLA, projectData.SvfUrl);
        }
    }
}
