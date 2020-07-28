﻿/////////////////////////////////////////////////////////////////////
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
    /// Generate PNG thumbnail for Inventor document.
    /// </summary>
    public class CreateThumbnail : ForgeAppBase
    {
        public override string Id => nameof(CreateThumbnail);
        public override string Description => "Generate thumbnail from Inventor document";

        protected override string OutputUrl(ProcessingArgs projectData)
        {
            return (projectData as AdoptionData).ThumbnailUrl; // TODO: use generics
        }

        protected override string OutputName => "thumbnail.png";

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateThumbnail(Publisher publisher) : base(publisher) { }
    }
}
