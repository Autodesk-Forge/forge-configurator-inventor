using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class CreateThumbnail : SimpleDocProcessing
    {
        public override string Id => nameof(CreateThumbnail);
        public override string Description => "Generate thumbnail from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            return projectData.SvfUrl;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateThumbnail(Publisher publisher) : base(publisher) { }

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
                        LocalName = "thumbnail.png",
                        Description = "Resulting thumbnail",
                        Ondemand = false,
                        Required = false
                    }
                }
            };
    }
}
