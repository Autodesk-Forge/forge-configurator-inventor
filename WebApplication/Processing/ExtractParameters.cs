using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Extract parameters from Inventor document.
    /// </summary>
    public class ExtractParameters : ForgeAppBase
    {
        private const string OutputModelParameterName = "OutputModelFile";

        public override string Id => nameof(ExtractParameters);
        public override string Description => "Extract Parameters and Save Inventor document";

        protected override string OutputUrl(ProcessingArgs projectData)
        {
            return projectData.ParametersJsonUrl;
        }

        protected override string OutputName => "documentParams.json";

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParameters(Publisher publisher) : base(publisher) { }

        protected override void AddOutputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            base.AddOutputArgs(args, data);
            args.Add(OutputModelParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.ModelUrl });

        }

        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var activityParams = base.GetActivityParams();
            activityParams.Add(OutputModelParameterName,
                new Parameter { Verb = Verb.Put, LocalName = "ModelCopy", Zip = true });
            return activityParams;
        }
    }
}
