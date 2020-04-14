using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class ExtractParametersDefinition : ForgeAppConfigBase
    {
        public override int EngineVersion => 24;

        public override string Id => "ExtractParameters";
        public override string Label => "alpha";
        public override string Description => "Extract Parameters from Inventor document";

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParametersDefinition(Publisher publisher) : base(publisher) { }

        internal static class Parameters
        {
            public static string InventorDoc = nameof(InventorDoc);
            public static string OutputJson = nameof(OutputJson);
        }

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) /i $(args[{Parameters.InventorDoc}].path)"
            };

        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
            {
                {
                    Parameters.InventorDoc,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "Inventor document file to process"
                    }
                },
                {
                    Parameters.OutputJson,
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "documentParams.json",
                        Description = "Resulting JSON",
                        Ondemand = false,
                        Required = false
                    }
                }
            };

        /// <summary>
        /// Process IPT file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="outputUrl"></param>
        public Task<WorkItemStatus> ProcessDocument(string url, string outputUrl)
        {
            var args = new Dictionary<string, IArgument>
                            {
                                {
                                    Parameters.InventorDoc,
                                    new XrefTreeArgument { Url = url }
                                },
                                {
                                    Parameters.OutputJson,
                                    new XrefTreeArgument { Verb = Verb.Put, Url = outputUrl }
                                }
                            };

            return Run(args);
        }
    }
}
