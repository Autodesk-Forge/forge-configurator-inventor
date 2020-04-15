using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    /// <summary>
    /// Preprocess incoming project:
    /// - generate thumbnail and SVF
    /// - extract parameters
    /// </summary>
    public class AdoptProjectDefinition : ForgeAppConfigBase
    {
        private readonly List<ForgeAppConfigBase> _definitions;

        public AdoptProjectDefinition(Publisher publisher) : base(publisher)
        {
            _definitions = new List<ForgeAppConfigBase>
                            {
                                new CreateSvfDefinition(publisher),
                                new CreateThumbnailDefinition(publisher),
                                new ExtractParametersDefinition(publisher)
                            };
        }

        public override int EngineVersion => _definitions[0].EngineVersion;
        public override string Label => _definitions[0].Label;

        public override string Id => "AdoptProjectDefinition";
        public override string Description => "Adopt Inventor project";

        public override bool HasBundle => false;

        public override List<string> ActivityCommandLine
        {
            get
            {
                // concat command lines from all definitions
                return _definitions.SelectMany(def => def.ActivityCommandLine).ToList();
            }
        }

        public override Dictionary<string, Parameter> ActivityParams
        {
            get
            {
                // merge parameters into a single dictionary
                var output = new Dictionary<string, Parameter>();
                foreach (var definition in _definitions)
                {
                    foreach (var (name, param) in definition.ActivityParams)
                    {
                        // avoid collisions
                        output[name] = param;
                    }
                }

                return output;
            }
        }

        protected override Dictionary<string, IArgument> ToIptArguments(string url, string outputUrl)
        {
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, IArgument> ToIamArguments(string url, string pathInZip, string outputUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}
