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
        private const string OutputModelParameterName = "OutputModelFile";
        private readonly bool _assembly;

        public AdoptProject(Publisher publisher, bool assembly) :
            base(publisher,
                    new CreateSVF(publisher),
                    new CreateThumbnail(publisher),
                    new ExtractParameters(publisher))
        {
            this._assembly = assembly;
        }

        public override string Id => $"{nameof(AdoptProject)}{(this._assembly ? "A" : "P")}";
        public override string Description => "Adopt Inventor project";
        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var parameters = base.GetActivityParams();
            bool zip = _assembly;
            string localName = _assembly ? FolderToBeZippedName : IptName;
            parameters.Add(OutputModelParameterName, new Parameter { Verb = Verb.Put, Zip = zip, LocalName = localName });
            return parameters;
        }
        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var args = base.ToWorkItemArgs(data);
            string localName = data.IsAssembly ? FolderToBeZippedName : IptName;
            args.Add(OutputModelParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputModelUrl});
            return args;
        }
    }
}
