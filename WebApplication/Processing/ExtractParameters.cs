using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class ExtractParameters : ForgeAppBase
    {
        public override string Id => nameof(ExtractParameters);
        public override string Description => "Extract Parameters from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            return projectData.ParametersJsonUrl;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParameters(Publisher publisher) : base(publisher) { }

        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
            {
                {
                    InputParameterName,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "Inventor document file to process"
                    }
                },
                {
                    OutputParameterName,
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "documentParams.json",
                        Description = "Resulting JSON",
                        Ondemand = false,
                        Required = false
                    }
                }
            };
    }
}
