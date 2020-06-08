using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// SVF generator from Inventor document.
    /// </summary>
    public class CreateSVF : ForgeAppBase
    {
        public override string Id => nameof(CreateSVF);
        public override string Description => "Generate SVF from Inventor document";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.SvfUrl;
        protected override string OutputName => "SvfOutput";
        protected override bool IsOutputZip => true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSVF(Publisher publisher) : base(publisher) {}
    }
}
