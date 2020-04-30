using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Extract parameters from Inventor document.
    /// </summary>
    public class ExtractParameters : ForgeAppBase
    {
        public override string Id => nameof(ExtractParameters);
        public override string Description => "Extract Parameters from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            return projectData.ParametersJsonUrl;
        }

        protected override string OutputName => "documentParams.json";

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParameters(Publisher publisher) : base(publisher) { }
    }
}
