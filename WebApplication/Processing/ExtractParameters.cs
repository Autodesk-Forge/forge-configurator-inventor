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
        private const string OutputModelParameterName = "OutputModelFile";

        public override string Id => nameof(ExtractParameters);
        public override string Description => "Extract Parameters and Save Inventor document";

        protected override string OutputName => "documentParams.json";
        protected override string OutputUrl(ProcessingArgs projectData)
        {
            return projectData.ParametersJsonUrl;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParameters(Publisher publisher) : base(publisher) { }

        protected override void AddOutputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            base.AddOutputArgs(args, data);
            args.Add(OutputModelParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputModelUrl });

        }

        public override Dictionary<string, Parameter> GetActivityParams()
        {
            Dictionary<string, Parameter> activityParams = base.GetActivityParams();
            activityParams.Add(OutputModelParameterName, new Parameter { Verb = Verb.Put, LocalName = "ModelCopy", Zip = true });
            return activityParams;
        }
    }
}
