using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// SAT generator from Inventor document.
    /// </summary>
    public class CreateSAT : ForgeAppBase
    {
        public override string Id => nameof(CreateSAT);
        public override string Description => "Generate SAT from Inventor document";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.SatUrl;
        protected override string OutputName => "export.sat";

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSAT(Publisher publisher) : base(publisher) {}
    }
}
