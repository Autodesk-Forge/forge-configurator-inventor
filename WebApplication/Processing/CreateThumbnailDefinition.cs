using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class CreateThumbnailDefinition : ForgeAppConfigBase
    {
        public override int EngineVersion => 24;
        public override string Id => "CreateThumbnail";
        public override string Label => "alpha";
        public override string Description => "Generate thumbnail from Inventor document";

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateThumbnailDefinition(Publisher publisher) : base(publisher) { }

        internal static class Parameters
        {
            public static string InventorDoc = nameof(InventorDoc);
            public static string OutputThumbnail = nameof(OutputThumbnail);
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
                    Parameters.OutputThumbnail,
                    new Parameter
                    {
                        Verb = Verb.Put,
                        LocalName = "thumbnail.png",
                        Description = "Resulting thumbnail",
                        Ondemand = false,
                        Required = false
                    }
                }
            };

        protected override Dictionary<string, IArgument> ToIptArguments(string url, string outputUrl)
        {
            return new Dictionary<string, IArgument>
            {
                {
                    Parameters.InventorDoc,
                    new XrefTreeArgument {Url = url}
                },
                {
                    Parameters.OutputThumbnail,
                    new XrefTreeArgument {Verb = Verb.Put, Url = outputUrl}
                }
            };
        }

        protected override Dictionary<string, IArgument> ToIamArguments(string url, string pathInZip, string outputUrl)
        {
            return new Dictionary<string, IArgument>
            {
                {
                    Parameters.InventorDoc,
                    new XrefTreeArgument {PathInZip = pathInZip, LocalName = "zippedIam.zip", Url = url}
                },
                {
                    Parameters.OutputThumbnail,
                    new XrefTreeArgument {Verb = Verb.Put, Url = outputUrl}
                }
            };
        }
    }
}
