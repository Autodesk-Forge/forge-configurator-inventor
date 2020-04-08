using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Utilities;

namespace IoConfigDemo
{
    internal class CreateSvfDefinition : ForgeAppConfigBase
    {
        public override int EngineVersion => 23;
        public override string Description => "Generate SVF from Inventor document";
        public override string Id => "CreateSvfForInventor";
        public override string Label => "alpha";

        internal static class Parameters
        {
            public static string InventorDoc = nameof(InventorDoc);
            public static string OutputIpt = nameof(OutputIpt);
        }

        /// <summary>
        /// Get command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{Parameters.InventorDoc}].path)"
            };

        /// <summary>
        /// Get activity parameters.
        /// </summary>
        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
            {
                {
                    Parameters.InventorDoc,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "IPT file to process"
                    }
                },
                {
                    Parameters.OutputIpt,
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "result.ipt",
                        Description = "Resulting IPT",
                        Ondemand = false,
                        Required = false
                    }
                }
            };

        /// <summary>
        /// Get arguments for workitem.
        /// </summary>
        public override Dictionary<string, IArgument> WorkItemArgs =>
            new Dictionary<string, IArgument>
            {
                {
                    Parameters.InventorDoc,
                    new XrefTreeArgument
                    {
                        Url = "!!! CHANGE ME !!!"
                    }
                },
                {
                    Parameters.OutputIpt,
                    new XrefTreeArgument
                    {
                        Verb = Verb.Put,
                        Url = "!!! CHANGE ME !!!"
                    }
                }
            };
    }
}