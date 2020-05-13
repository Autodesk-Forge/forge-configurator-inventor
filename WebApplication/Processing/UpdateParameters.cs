using System;
using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class UpdateParameters : ForgeAppBase
    {
        public override string Id => nameof(UpdateParameters);
        public override string Description => "Update parameters from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            throw new NotImplementedException();
        }

        protected override string OutputName => "documentParams.json";

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{InputParameterName}].path) $(args[InventorParams].path)"
            };

        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
            {
                {
                    InputParameterName,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "IPT or IAM (in ZIP) file to process"
                    }
                },
                {
                    "InventorParams",
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "JSON file with Inventor parameters"
                    }
                },
                {
                    OutputParameterName,
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = OutputName,
                        Zip = IsOutputZip
                    }
                }
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateParameters(Publisher publisher) : base(publisher)
        {
        }
    }
}
