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
    /// Update all drawings.
    /// </summary>
    public class UpdateDrawings : ForgeAppBase
    {
        public override string Id => nameof(UpdateDrawings);
        public override string Description => "Find all drawings and update them -> zip";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.DrawingUrl;
        protected override string OutputName => "drawing.zip";
        protected override bool IsOutputZip => false;
        protected override bool IsOutputOptional => true;

        /// <summary>
        /// Parameter name for input drawing.
        /// </summary>
        public const string InputDrawingParameterName = "InventorDrawing";

        protected const string FolderToBeZippedDrawingName = "unzippedDrawing";

        protected override void AddInputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            if (data.IsAssembly)
            {
                args.Add(InputDocParameterName, new XrefTreeArgument { PathInZip = data.TLA, LocalName = FolderToBeZippedName, Url = data.InputDocUrl });
                if (data.InputDrawingUrl != null)
                {
                    args.Add(InputDrawingParameterName, new XrefTreeArgument { PathInZip = "NeedSomeValueForCorrectUnzip", LocalName = FolderToBeZippedDrawingName, Url = data.InputDrawingUrl, Optional = true });
                }
            }
        }

        /// <summary>
        /// Command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{ActivityId}].path) \"$(args[{InputDocParameterName}].path)\" \"$(args[{InputDrawingParameterName}].path)\""
            };

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
                    InputDrawingParameterName,
                    new Parameter
                    {
                        Verb = Verb.Get,
                        Description = "Drawings (in ZIP) file to process"
                    }
                }
            };

            if (HasOutput)
            {
                activityParams.Add(OutputParameterName,
                    new Parameter { Verb = Verb.Put, LocalName = OutputName, Zip = IsOutputZip });
            }

            return activityParams;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateDrawings(Publisher publisher) : base(publisher) {}
    }
}
