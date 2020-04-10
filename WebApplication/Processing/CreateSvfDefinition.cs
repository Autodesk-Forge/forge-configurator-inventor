using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    internal class CreateSvfDefinition : ForgeAppConfigBase
    {
        public override int EngineVersion => 24;

        public override string Id => "CreateSVF";
        public override string Label => "alpha";
        public override string Description => "Generate SVF from Inventor document";

        internal static class Parameters
        {
            public static string InventorDoc = nameof(InventorDoc);
            public static string OutputZip = nameof(OutputZip);
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
                    Parameters.OutputZip,
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "SvfOutput",
                        Description = "Resulting files with SVF",
                        //Ondemand = false,
                        //Required = false,
                        Zip = true
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
                    Parameters.OutputZip,
                    new XrefTreeArgument
                    {
                        Verb = Verb.Put,
                        Url = "!!! CHANGE ME !!!"
                    }
                }
            };

        public async Task ProcessIPT(Publisher publisher, string url, string outputUrl)
        {
            var args = new Dictionary<string, IArgument>
                            {
                                {
                                    Parameters.InventorDoc,
                                    new XrefTreeArgument
                                    {
                                        Url = url
                                    }
                                },
                                {
                                    Parameters.OutputZip,
                                    new XrefTreeArgument
                                    {
                                        Verb = Verb.Put,
                                        Url = outputUrl
                                    }
                                }
                            };

            await publisher.RunWorkItemAsync(args);
        }
    }
}