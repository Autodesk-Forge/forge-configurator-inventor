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

using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Drawings List.
    /// </summary>
    public class DrawingsList : ForgeAppBase
    {
        public override string Id => nameof(DrawingsList);
        public override string Description => "Gets list of drawings in the package";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.DrawingsListUrl;
        protected override string OutputName => "drawingsList.json";
        protected override bool IsOutputZip => false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawingsList(Publisher publisher) : base(publisher) {}
    }
}
