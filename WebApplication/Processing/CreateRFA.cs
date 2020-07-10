using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Generic;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// RFA generator from Inventor document.
    /// </summary>
    public class CreateRFA : ForgeAppBase
    {
        public override string Engine { protected set; get; } = "Autodesk.Revit+2020";

        public override string Id => nameof(CreateRFA);
        public override string Description => "Generate RFA from SAT document";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.RfaUrl;
        protected override string OutputName => "Output.rfa";

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\revitcoreconsole.exe /al \"$(appbundles[{ActivityId}].path)\""
            };

        protected override void AddInputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            args.Add(InputDocParameterName, new XrefTreeArgument { Url = data.InputDocUrl, LocalName = "Input.sat" });
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateRFA(Publisher publisher) : base(publisher) {}
    }
}
