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
    public class ExportDrawing : ForgeAppBase
    {
        private const string DrawingParameter = "DrawingParameter";
        public override string Id => nameof(ExportDrawing);
        public override string Description => "Find the drawing of Inventor document and generate viewables for ForgeViewer";

        protected internal override ForgeRegistration Registration { get; } = ForgeRegistration.All;

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.DrawingPdfUrl!;
        protected override string OutputName => "Drawing.pdf";
        protected override bool IsOutputZip => false;
        protected override bool IsOutputOptional => true;

        /// <summary>
        /// Command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new()
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al \"$(appbundles[{ActivityId}].path)\" \"$(args[{InputDocParameterName}].path)\" \"$(args[{DrawingParameter}].value)\""
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExportDrawing(Publisher publisher) : base(publisher) {}

        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var activityParams = base.GetActivityParams();

            activityParams.Add(DrawingParameter, new Parameter { Verb = Verb.Read, Description = "Drawing to generate" });

            return activityParams;
        }

        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var workItemArgs = base.ToWorkItemArgs(data);

            DrawingPdfData projectData = (data as DrawingPdfData)!;
            workItemArgs.Add(DrawingParameter, new StringArgument { Value = projectData.DrawingToGenerate });

            return workItemArgs;
        }
    }
}
