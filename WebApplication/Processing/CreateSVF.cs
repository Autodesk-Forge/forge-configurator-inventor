using System.Collections.Generic;
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

        protected override string OutputUrl(AdoptionData projectData)
        {
            return projectData.SvfUrl;
        }

        protected override string OutputName => "SvfOutput";
        protected override bool IsOutputZip => true;

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /p /i $(args[{InputParameterName}].path)"
            };


        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSVF(Publisher publisher) : base(publisher) {}
    }
}
