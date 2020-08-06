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

namespace WebApplication.Definitions
{
    //TODO: split the update urls and rfa urls
    public class ProcessingArgs
    {
        public string InputDocUrl { get; set; }

        /// <summary>
        /// Relative path to top level assembly in ZIP with assembly.
        /// </summary>
        public string TLA { get; set; }

        public string SvfUrl { get; set; }
        public string ParametersJsonUrl { get; set; }

        /// <summary>
        /// If job data contains assembly.
        /// </summary>
        public bool IsAssembly => ! string.IsNullOrEmpty(TLA);

        public string OutputIAMModelUrl { get; set; }
        public string OutputIPTModelUrl { get; set; }
        public string SatUrl { get; internal set; }
        public string RfaUrl { get; internal set; }
        public string BomUrl { get; set; }
    }

    /// <summary>
    /// All data required for project adoption.
    /// </summary>
    public class AdoptionData : ProcessingArgs
    {
        public string ThumbnailUrl { get; set; }
    }

    /// <summary>
    /// All data required for project update.
    /// </summary>
    public class UpdateData : ProcessingArgs
    {
        public string InputParamsUrl { get; set; }
    }
}
