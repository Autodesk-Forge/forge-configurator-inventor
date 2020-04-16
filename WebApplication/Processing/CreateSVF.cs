using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class CreateSVF : SimpleDocProcessing
    {
        public override string Id => nameof(CreateSVF);
        public override string Description => "Generate SVF from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            return projectData.SvfUrl;
        }

        /// <summary>
        /// Get activity parameters.
        /// </summary>
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
                    OutputParameterName, 
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "SvfOutput",
                        Description = "Resulting files with SVF",
                        Zip = true
                    }
                }
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSVF(Publisher publisher) : base(publisher) {}
    }
}
