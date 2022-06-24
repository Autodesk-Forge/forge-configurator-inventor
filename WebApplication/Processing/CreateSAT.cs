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
    /// SAT generator from Inventor document.
    /// </summary>
    public class CreateSAT : ForgeAppBase
    {
        public override string Id => nameof(CreateSAT);
        public override string Description => "Generate SAT from Inventor document";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.SatUrl;
        protected override string OutputName => "export.sat";

        protected internal override ForgeRegistration Registration { get; } = ForgeRegistration.All;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateSAT(Publisher publisher) : base(publisher) {}
    }
}
