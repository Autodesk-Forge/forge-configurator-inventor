/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Inventor;

namespace SplitDrawingsPlugin
{
    [ComVisible(true)]
    public class SplitDrawingsAutomation
    {
        private readonly InventorServer inventorApplication;

        public SplitDrawingsAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogError("Run is not functional");
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            var dir = System.IO.Directory.GetCurrentDirectory();
            var key = map.Item["_1"];

            string rootDir = System.IO.Path.Combine(dir, key);

            LogTrace("Processing directory: " + rootDir);

            // search if there is some drawings
            var drawingExtensions = new List<string> { ".idw", ".dwg" };
            var skipExtensions = new List<string> { ".idw", ".dwg", ".lck", ".zip" };
            string[] drawings = System.IO.Directory.GetFiles(rootDir, "*.*", System.IO.SearchOption.AllDirectories)
                                .Where(file => drawingExtensions.IndexOf(System.IO.Path.GetExtension(file)) >= 0).ToArray();
            string[] allExceptDrawingFiles = System.IO.Directory.GetFiles(rootDir, "*.*", System.IO.SearchOption.AllDirectories)
                                .Where(file => skipExtensions.IndexOf(System.IO.Path.GetExtension(file)) == -1).ToArray();

            if (drawings.Length == 0)
            {
                LogTrace("No drawings found.");
                return;
            }

            LogTrace("drawings:");
            foreach (var dr in drawings)
                LogTrace(dr);
            LogTrace("files:");
            foreach (var f in allExceptDrawingFiles)
                LogTrace(f);

            var modelFileName = System.IO.Path.Combine(dir, "model.zip");
            var drawingFileName = System.IO.Path.Combine(dir, "drawing.zip");

            using (var modelFS = new FileStream(modelFileName, FileMode.Create))
            using (var zip = new ZipArchive(modelFS, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in allExceptDrawingFiles)
                {
                    var pathInArchive = filePath.Substring(rootDir.Length+1);
                    ZipArchiveEntry newEntry = zip.CreateEntry(pathInArchive);
                    using (var entryStream = newEntry.Open())
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    using (var fileMS = new MemoryStream())
                    using (var writer = new BinaryWriter(entryStream, Encoding.UTF8))
                    {
                        fileStream.CopyTo(fileMS);
                        writer.Write(fileMS.ToArray());
                    }
                }
            }
            LogTrace($"Created model.zip, {allExceptDrawingFiles.Length} item(s).");

            using (var drawingFS = new FileStream(drawingFileName, FileMode.Create))
            using (var zip = new ZipArchive(drawingFS, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in drawings)
                {
                    var pathInArchive = filePath.Substring(rootDir.Length+1);
                    ZipArchiveEntry newEntry = zip.CreateEntry(pathInArchive);
                    using (var entryStream = newEntry.Open())
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    using (var fileMS = new MemoryStream())
                    using (var writer = new BinaryWriter(entryStream, Encoding.UTF8))
                    {
                        fileStream.CopyTo(fileMS);
                        writer.Write(fileMS.ToArray());
                    }
                }
            }
            LogTrace($"Created drawing.zip, {drawings.Length} item(s).");
        }

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}