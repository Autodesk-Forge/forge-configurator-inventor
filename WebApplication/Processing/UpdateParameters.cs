using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class UpdateParameters : ForgeAppBase
    {
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
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{InputParameterName}].path) $(args[{InventorParameters}].path)"
            };

        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
            {
                {
                    InputParameterName,
                    new Parameter { Verb = Verb.Get, Description = "IPT or IAM (in ZIP) file to process" }
                },
                {
                    InventorParameters,
                    new Parameter { Verb = Verb.Get, Description = "JSON file with Inventor parameters" }
                }
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateParameters(Publisher publisher) : base(publisher)
        {
        }

        public override Dictionary<string, IArgument> ToWorkItemArgs(AdoptionData projectData)
        {
            var workItemArgs = base.ToWorkItemArgs(projectData);
            workItemArgs.Add(InventorParameters, new XrefTreeArgument { Url = projectData.InputParamsUrl });
            return workItemArgs;
        }
    }
}
