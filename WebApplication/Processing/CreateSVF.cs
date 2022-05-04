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

using webapplication.Definitions;

namespace webapplication.Processing
{
    /// <summary>
    /// SVF generator from Inventor document.
    /// </summary>
    public class CreateSVF : ForgeAppBase
    {
        public override string Id => nameof(CreateSVF);
        public override string Description => "Generate SVF from Inventor document";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.SvfUrl;
        protected override string OutputName => "SvfOutput";
        protected override bool IsOutputZip => true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSVF(Publisher publisher) : base(publisher) {}
    }
}
