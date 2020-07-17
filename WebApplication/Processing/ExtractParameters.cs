using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Extract parameters from Inventor document and save the current model state.
    /// </summary>
    public class ExtractParameters : ForgeAppBase
    {
        

        public override string Id => nameof(ExtractParameters);
        public override string Description => "Extract Parameters and Save Inventor document";

        protected override string OutputName => "documentParams.json";
        protected override string OutputUrl(ProcessingArgs projectData) => projectData.ParametersJsonUrl;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParameters(Publisher publisher) : base(publisher) { }
    }
}
