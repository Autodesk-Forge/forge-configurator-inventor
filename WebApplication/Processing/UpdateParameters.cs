using System;
using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class UpdateParameters : ForgeAppBase
    {
        private const string InventorParameters = "InventorParams";
        public override string Id => nameof(UpdateParameters);
        public override string Description => "Update parameters from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            return "https://developer.api.autodesk.com/oss/v2/signedresources/12345678-3e09-449d-8fd6-b1d2c3b711d3?region=US";
        }

        protected override string OutputName => "documentParams.json";

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{InputParameterName}].path) $(args[InventorParams].path)"
            };

        public override Dictionary<string, Parameter> ActivityParams
        {
            get
            {
                var parameters = base.ActivityParams;
                parameters.Add(InventorParameters, new Parameter { Verb = Verb.Get,  Description = "JSON file with Inventor parameters" });
                return parameters;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateParameters(Publisher publisher) : base(publisher)
        {
        }

        public override Dictionary<string, IArgument> ToWorkItemArgs(AdoptionData projectData)
        {
            var workItemArgs = base.ToWorkItemArgs(projectData);
            workItemArgs.Add(InventorParameters, new XrefTreeArgument { Url = "https://developer.api.autodesk.com/oss/v2/signedresources/385475d6-3e09-449d-8fd6-b1d2c3b711d3?region=US" });
            return workItemArgs;
        }
    }
}
