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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Compression;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UpdateDrawingsPlugin
{
    [ComVisible(true)]
    public class UpdateDrawingsAutomation
    {
        private readonly InventorServer inventorApplication;

        public UpdateDrawingsAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace("Processing " + doc.FullFileName);

            try
            {
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }

        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            using (new HeartBeat())
            {
                var rootDir = System.IO.Directory.GetCurrentDirectory();
                if (doc == null)
                {
                    ActivateDefaultProject(rootDir);
                    //doc = inventorApplication.Documents.Open(map.Item["_1"]);
                }

                var drawingExtensions = new List<string> { ".idw", ".dwg" };
                var oldVersion = @"OldVersions\";
                string[] drawings = System.IO.Directory.GetFiles(rootDir, "*.*", System.IO.SearchOption.AllDirectories)
                                    .Where(file => drawingExtensions.IndexOf(System.IO.Path.GetExtension(file)) >= 0 &&
                                    !file.Contains(oldVersion)).ToArray();

                foreach (var filePath in drawings)
                {
                    LogTrace($"Updating drawing {filePath}");
                    var drawingDocument = inventorApplication.Documents.Open(filePath);
                    LogTrace("Drawing opened");
                    drawingDocument.Update2(true);
                    LogTrace("Drawing updated");
                    drawingDocument.Save2(true);
                    LogTrace("Drawing saved");
                }

                // zip updated drawings
                var drawingFileName = System.IO.Path.Combine(rootDir, "drawing.zip");

                using (var drawingFS = new FileStream(drawingFileName, FileMode.Create))
                using (var zip = new ZipArchive(drawingFS, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in drawings)
                    {
                        var pathInArchive = filePath.Substring(rootDir.Length);
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
        }

        private void ActivateDefaultProject(string dir)
        {
            var defaultProjectName = "FDADefault";

            var projectFullFileName = System.IO.Path.Combine(dir, defaultProjectName + ".ipj");

            DesignProject project = null;
            if (System.IO.File.Exists(projectFullFileName))
            {
                project = inventorApplication.DesignProjectManager.DesignProjects.AddExisting(projectFullFileName);
                Trace.TraceInformation("Adding existing default project file: {0}", projectFullFileName);

            }
            else
            {
                project = inventorApplication.DesignProjectManager.DesignProjects.Add(MultiUserModeEnum.kSingleUserMode, defaultProjectName, dir);
                Trace.TraceInformation("Creating default project file with name: {0} at {1}", defaultProjectName, dir);
            }

            Trace.TraceInformation("Activating default project {0}", project.FullFileName);
            project.Activate(true);
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