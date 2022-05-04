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

namespace webapplication.Definitions
{
    /// <summary>
    /// Common pieces for project-related DTOs
    /// </summary>
    public class ProjectDTOBase
    {
        /// <summary>
        /// URL to SVF directory.
        /// </summary>
        public string Svf { get; set; }

        /// <summary>
        /// URL to BOM JSON.
        /// </summary>
        public string BomJsonUrl { get; set; }

        /// <summary>
        /// URL to download BOM CSV.
        /// </summary>
        public string BomDownloadUrl { get; set; }

        /// <summary>
        /// URL to download current model
        /// </summary>
        public string ModelDownloadUrl { get; set; }

        /// <summary>
        /// Parameters hash
        /// </summary>
        public string Hash { get; set; }
    }
}
