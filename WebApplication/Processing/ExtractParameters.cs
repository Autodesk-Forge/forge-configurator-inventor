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
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Extract parameters from Inventor document and save the current model state.
    /// </summary>
    public class ExtractParameters : ForgeAppBase
    {
        

        public override string Id => nameof(ExtractParameters);
        public override string Description => "Extract Parameters and Save Inventor document";

        protected override string OutputName => "documentParams.json";
        protected override string OutputUrl(ProcessingArgs projectData) => projectData.ParametersJsonUrl;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtractParameters(Publisher publisher) : base(publisher) { }
    }
}
