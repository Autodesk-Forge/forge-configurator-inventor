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

using System;
using Shared;

namespace WebApplication.Definitions
{
    public class ProjectDTO : ProjectDTOBase
    {
        public string Id { get; set; }
        public string Label { get; set; }

        /// <summary>
        /// Thumbnail URL.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// If project is assembly.
        /// </summary>
        public bool IsAssembly { get; set; }
        
        /// <summary>
        /// If project has drawings
        /// </summary>
        [Obsolete]
        public bool HasDrawing { get; set; }

        /// <summary>
        /// URL to DrawingsList JSON.
        /// </summary>
        public string DrawingsListUrl { get; set; }

        /// <summary>
        /// Adoption messages.
        /// </summary>
        public string[] AdoptWarnings { get; set; }
    }
}
