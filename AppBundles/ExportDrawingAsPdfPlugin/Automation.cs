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

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;

namespace ExportDrawingAsPdfPlugin
{
    [ComVisible(true)]
    public class Automation
    {
        private readonly InventorServer inventorApplication;

        public Automation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            


        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            using (new HeartBeat())
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                if (doc == null)
                {
                    ActivateDefaultProject(dir);
                    doc = inventorApplication.Documents.Open(map.Item["_1"]);
                }
                var fullFileName = doc.FullFileName;
                var path = System.IO.Path.GetFullPath(fullFileName);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(fullFileName);
                var drawing = inventorApplication.DesignProjectManager.ResolveFile(path, fileName + ".idw");
                LogTrace("Looking for drawing: " + fileName + ".idw " + "inside: " + path + " with result: " + drawing);
                if (drawing == null)
                {
                    drawing = inventorApplication.DesignProjectManager.ResolveFile(path, fileName + ".dwg");
                    LogTrace("Looking for drawing: " + fileName + ".dwg " + "inside: " + path + " with result: " + drawing);
                }

                if (drawing != null)
                {
                    LogTrace("Found drawing to export at: " + drawing);
                    var drawingDocument = inventorApplication.Documents.Open(drawing);
                    LogTrace("Drawing opened");
                    drawingDocument.Update2(true);
                    LogTrace("Drawing updated");
                    drawingDocument.Save2(true);
                    LogTrace("Drawing saved");
                    var pdfPath = System.IO.Path.Combine(dir, "Drawing.pdf");
                    LogTrace("Exporting drawing to: " + pdfPath);
                    drawingDocument.SaveAs(pdfPath, true);
                    LogTrace("Drawing exported");
                }
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

            } else
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
        private static void LogTrace(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

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
        private static void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
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