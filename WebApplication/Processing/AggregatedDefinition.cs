using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Special definition for aggregated activities, where all bundles are "external".
    /// </summary>
    public abstract class AggregatedDefinition : ForgeAppBase
    {
        protected ForgeAppBase[] Definitions => _definitions;
        private readonly ForgeAppBase[] _definitions;

        protected AggregatedDefinition(Publisher publisher, params ForgeAppBase[] items) : base(publisher)
        {
            if (items.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(items));

            _definitions = items;
        }

        public override int EngineVersion => _definitions[0].EngineVersion;
        public override string Label => _definitions[0].Label;
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
                var all = _definitions.Select(def => def.ActivityParams);

                return Collections.MergeDictionaries(all);
            }
        }

        public override Dictionary<string, IArgument> ToIptArguments(string url, string outputUrl)
        {
            var all = _definitions.Select(def => def.ToIptArguments(url, outputUrl));
            return Collections.MergeDictionaries(all);
        }

        public override Dictionary<string, IArgument> ToIamArguments(string url, string pathInZip, string outputUrl)
        {
            var all = _definitions.Select(def => def.ToIamArguments(url, pathInZip, outputUrl));
            return Collections.MergeDictionaries(all);
        }

        public override List<string> GetBundles(string nickname)
        {
            return _definitions.SelectMany(def => def.GetBundles(nickname)).ToList();
        }
    }
}
