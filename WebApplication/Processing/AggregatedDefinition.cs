using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Special definition for aggregated activities, where all bundles are "external".
    /// </summary>
    public abstract class AggregatedDefinition : ForgeAppBase
    {
        private readonly ForgeAppBase[] _definitions;

        protected AggregatedDefinition(Publisher publisher, params ForgeAppBase[] items) : base(publisher)
        {
            if (items.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(items));

            _definitions = items;
        }

        public override bool HasBundle => false;

        public override List<string> ActivityCommandLine
        {
            get
            {
                // concat command lines from all definitions
                return _definitions.SelectMany(def => def.ActivityCommandLine).ToList();
            }
        }

        public override Dictionary<string, Parameter> GetActivityParams()
        {
            // merge parameters into a single dictionary
            var all = _definitions.Select(def => def.GetActivityParams());
            return Collections.MergeDictionaries(all);
        }

        protected override string OutputName => throw new NotImplementedException();

        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var all = _definitions.Select(def => def.ToWorkItemArgs(data));
            return Collections.MergeDictionaries(all);
        }

        public override List<string> GetBundles(string nickname)
        {
            return _definitions.SelectMany(def => def.GetBundles(nickname)).ToList();
        }

        protected override string OutputUrl(ProcessingArgs projectData)
        {
            throw new NotImplementedException();
        }
    }
}
