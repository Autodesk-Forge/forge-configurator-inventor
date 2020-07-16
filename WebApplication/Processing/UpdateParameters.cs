using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class UpdateParameters : ForgeAppBase
    {
        private const string OutputModelIAMParameterName = "OutputModelIAMFile";
        private const string OutputModelIPTParameterName = "OutputModelIPTFile";
        /// <summary>
        /// Design Automation parameter name for JSON file with Inventor parameters.
        /// </summary>
        private const string InventorParameters = "InventorParams";

        public override string Id => nameof(UpdateParameters);
        public override string Description => "Update parameters from Inventor document";

        protected override bool HasOutput => false;

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i \"$(args[{InputDocParameterName}].path)\" \"$(args[{InventorParameters}].path)\" /p"
            };

        public override Dictionary<string, Parameter> GetActivityParams() =>
            new Dictionary<string, Parameter>
            {
                {
                    InputDocParameterName,
                    new Parameter { Verb = Verb.Get, Description = "IPT or IAM (in ZIP) file to process" }
                },
                {
                    InventorParameters,
                    new Parameter { Verb = Verb.Get, Description = "JSON file with Inventor parameters" }
                },
                {
                    OutputModelIAMParameterName,
                    new Parameter { Verb = Verb.Put, LocalName = FolderToBeZippedName, Zip = true }
                },
                {
                    OutputModelIPTParameterName,
                    new Parameter { Verb = Verb.Put, LocalName = IptName }
                }
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateParameters(Publisher publisher) : base(publisher) {}

        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var workItemArgs = base.ToWorkItemArgs(data);

            UpdateData projectData = data as UpdateData;
            if (projectData.InputParamsUrl != null) // TODO: use generics
            {
                workItemArgs.Add(InventorParameters, new XrefTreeArgument { Url = projectData.InputParamsUrl });
                workItemArgs.Add(OutputModelIAMParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIAMModelUrl, Optional = true });
                workItemArgs.Add(OutputModelIPTParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIPTModelUrl, Optional = true });
            }
            return workItemArgs;
        }
    }
}
