using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class CreateThumbnail : ForgeAppBase
    {
        public override int EngineVersion => 24;
        public override string Id => nameof(CreateThumbnail);
        public override string Label => "alpha";
        public override string Description => "Generate thumbnail from Inventor document";
        public override string OutputParameterName => "OutputThumbnail";

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateThumbnail(Publisher publisher) : base(publisher) { }

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
                    OutputParameterName,
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

        public override Dictionary<string, IArgument> ToIptArguments(AdoptionData projectData)
        {
            return ToIptArguments(projectData.InputUrl, projectData.ThumbnailUrl);
        }

        public override Dictionary<string, IArgument> ToIamArguments(AdoptionData projectData)
        {
            return ToIamArguments(projectData.InputUrl, projectData.TLA, projectData.ThumbnailUrl);
        }
    }
}
