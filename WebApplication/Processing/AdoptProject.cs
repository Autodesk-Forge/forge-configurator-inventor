using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Generic;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Preprocess incoming project:
    /// - generate thumbnail and SVF
    /// - extract parameters
    /// </summary>
    public class AdoptProject : AggregatedDefinition
    {
        private const string OutputModelIAMParameterName = "OutputModelIAMFile";
        private const string OutputModelIPTParameterName = "OutputModelIPTFile";

        public AdoptProject(Publisher publisher) :
            base(publisher,
                    new CreateSVF(publisher),
                    new CreateThumbnail(publisher),
                    new ExtractParameters(publisher))
        {}

        public override string Id => nameof(AdoptProject);
        public override string Description => "Adopt Inventor project";
        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var parameters = base.GetActivityParams();
            parameters.Add(OutputModelIAMParameterName, new Parameter { Verb = Verb.Put, Zip = true, LocalName = FolderToBeZippedName, Required = false });
            parameters.Add(OutputModelIPTParameterName, new Parameter { Verb = Verb.Put, LocalName = IptName, Required = false });
            return parameters;
        }
        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var args = base.ToWorkItemArgs(data);
            args.Add(OutputModelIAMParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIAMModelUrl, Optional = true });
            args.Add(OutputModelIPTParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIPTModelUrl, Optional = true });
            return args;
        }
    }
}
