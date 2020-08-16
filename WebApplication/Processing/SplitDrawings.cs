/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Generic;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Export drawing from Inventor document.
    /// </summary>
    public class SplitDrawings : ForgeAppBase
    {
        public override string Id => nameof(SplitDrawings);
        public override string Description => "Split drawing from source dataset and generate model.zip and drawing.zip";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.IsAssembly ? projectData.OutputIAMModelUrl : projectData.OutputIPTModelUrl;
        protected override string OutputName => "model.zip";
        protected string OutputDrawingUrl(ProcessingArgs projectData) => projectData.OutputDrawingUrl;
        protected string OutputDrawingName => "drawing.zip";
        protected override bool IsOutputZip => false;
        protected override bool IsOutputOptional => true;

        protected override void AddOutputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            args.Add(OutputParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = OutputUrl(data), Optional = IsOutputOptional });
            args.Add(Id + "OutputDrawing", new XrefTreeArgument { Verb = Verb.Put, Url = OutputDrawingUrl(data), Optional = IsOutputOptional });
        }

        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var activityParams = new Dictionary<string, Parameter>
            {
                {
                    InputDocParameterName,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "IPT or IAM (in ZIP) file to process"
                    }
                },
                {
                    OutputParameterName, 
                    new Parameter
                    {
                        Verb = Verb.Put, LocalName = OutputName, Zip = false
                    }
                },
                {
                    Id + "OutputDrawing",
                    new Parameter
                    {
                        Verb = Verb.Put, LocalName = OutputDrawingName, Zip = false
                    }
                }
            };

            return activityParams;
        }

        /// <summary>
        /// Command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) unzippedIam"
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplitDrawings(Publisher publisher) : base(publisher) {}
    }
}
