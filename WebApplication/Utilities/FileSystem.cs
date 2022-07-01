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
    /// Utilities for files/directories work.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Copy directory.
        /// </summary>
        public static void CopyDir(string dirFrom, string dirTo)
        {
            // based on https://stackoverflow.com/a/8022011
            var dirFromLength = dirFrom.Length + 1;

            foreach (string dir in Directory.GetDirectories(dirFrom, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(dirTo, dir.Substring(dirFromLength)));
            }

            foreach (string fileName in Directory.GetFiles(dirFrom, "*", SearchOption.AllDirectories))
            {
                File.Copy(fileName, Path.Combine(dirTo, fileName.Substring(dirFromLength)));
            }
        }
    }
}
