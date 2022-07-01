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
using System.IO;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Wrapper for temporary file, which is deleted on disposal.
    /// </summary>
    public class TempFile : IDisposable
    {
        /// <summary>
        /// Full filename of uniquely named, zero-byte temporary file on disk.
        /// </summary>
        public string Name { get; }

        public TempFile()
        {
            Name = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(Name);
        }
    }
}
