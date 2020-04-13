using System;
using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;

namespace WebApplication.Processing
{
    public class CreateThumbnailDefinition : ForgeAppConfigBase
    {
        public override int EngineVersion => 24;

        public override string Id => "CreateThumbnail";
        public override string Label => "alpha";
        public override string Description => "Generate thumbnail from Inventor document";

        public override List<string> ActivityCommandLine => throw new NotImplementedException();
        public override Dictionary<string, Parameter> ActivityParams => throw new NotImplementedException();

        public CreateThumbnailDefinition(Publisher publisher) : base(publisher) {}
    }
}
