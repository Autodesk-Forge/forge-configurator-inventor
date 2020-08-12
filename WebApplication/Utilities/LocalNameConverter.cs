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

using System.IO;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Convert relative filenames to fullnames.
    /// </summary>
    public class LocalNameConverter
    {
        public string BaseDir { get; }

        public LocalNameConverter(string baseDir)
        {
            BaseDir = baseDir;
        }

        /// <summary>
        /// Generate full local name for the filename.
        /// </summary>
        public string ToFullName(string fileName)
        {
            return Path.Combine(BaseDir, fileName);
        }
    }

    /// <summary>
    /// Local attribute files.
    /// </summary>
    public class LocalAttributes : LocalNameConverter
    {
        /// <summary>
        /// Filename for thumbnail image.
        /// </summary>
        public string Thumbnail => ToFullName(LocalName.Thumbnail);

        /// <summary>
        /// Filename of JSON file with project metadata.
        /// </summary>
        public string Metadata => ToFullName(LocalName.Metadata);

        public LocalAttributes(string rootDir, string projectDir) : base(Path.Combine(rootDir, projectDir))
        {
        }
    }

    /// <summary>
    /// Names for "hashed" files.
    /// </summary>
    public class LocalNameProvider : LocalNameConverter
    {
        /// <summary>
        /// Fullname of directory with SVF data.
        /// </summary>
        public string SvfDir => ToFullName(LocalName.SvfDir);

        /// <summary>
        /// Filename for JSON with Inventor document parameters.
        /// </summary>
        public string Parameters => ToFullName(LocalName.Parameters);

        /// <summary>
        /// Filename for JSON with BOM.
        /// </summary>
        public string BOM => ToFullName(LocalName.BOM);

        public string DrawingViewables => ToFullName(LocalName.DrawingViewables);

        public LocalNameProvider(string projectDir, string hash) : base(Path.Combine(projectDir, hash))
        {
        }
    }
}
