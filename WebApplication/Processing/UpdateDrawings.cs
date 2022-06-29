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

using System.Collections.Generic;
using webapplication.Definitions;

namespace webapplication.Processing
{
    /// <summary>
    /// Update all drawings.
    /// </summary>
    public class UpdateDrawings : ForgeAppBase
    {
        public override string Id => nameof(UpdateDrawings);
        public override string Description => "Find all drawings and update them -> zip";

        protected internal override ForgeRegistration Registration { get; } = ForgeRegistration.All;

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.DrawingUrl!;
        protected override string OutputName => "drawing";
        protected override bool IsOutputZip => true;
        protected override bool IsOutputOptional => true;

        /// <summary>
        /// Command line for activity.
        /// </summary>
        public override List<string> ActivityCommandLine =>
            new()
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al \"$(appbundles[{ActivityId}].path)\" \"$(args[{InputDocParameterName}].path)\" "
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateDrawings(Publisher publisher) : base(publisher) {}
    }
}
