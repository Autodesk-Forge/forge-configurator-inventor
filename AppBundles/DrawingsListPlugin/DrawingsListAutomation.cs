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
using System.Collections.Generic;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Autodesk.Forge.DesignAutomation.Inventor.Utils.Helpers;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace DrawingsListPlugin
{
    [ComVisible(true)]
    public class DrawingsListAutomation
    {
        private readonly InventorServer inventorApplication;

        public DrawingsListAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            RunWithArguments(doc, null);
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            using (new HeartBeat())
            {
                var rootDir = System.IO.Directory.GetCurrentDirectory();

                if (doc == null)
                {
                    doc = inventorApplication.Documents.Open(map.Item["_1"]);
                }

                var fullFileName = doc.FullFileName;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(fullFileName);

                var drawingExtensions = new List<string> { ".idw", ".dwg" };
                var oldVersion = @"oldversions\";
                var drawings = System.IO.Directory.GetFiles(rootDir, "*.*", System.IO.SearchOption.AllDirectories)
                                    .Where(file => drawingExtensions.IndexOf(System.IO.Path.GetExtension(file.ToLower())) >= 0 &&
                                    !file.ToLower().Contains(oldVersion));

                var index = System.IO.Path.Combine(rootDir, "unzippedIam").Length + 1;
                // order by filename
                drawings = drawings.Select(path => path.Substring(index)).OrderBy(d => System.IO.Path.GetFileName(d));

                var defaultDrawings = drawings.Where(d => System.IO.Path.GetFileNameWithoutExtension(d) == fileName).ToArray();
                var defaultDrawing = defaultDrawings.Length > 0 ? defaultDrawings[0] : null;
                LogTrace("DEFAULT drawing is: " + defaultDrawing);

                var outputDrawingsList = new List<string>();

                // the first one is default when found
                if (defaultDrawing != null)
                    outputDrawingsList.Add(defaultDrawing);

                // add the rest
                outputDrawingsList.AddRange(drawings.Where(d => defaultDrawing==null || d != defaultDrawing));

                using (StreamWriter file = System.IO.File.CreateText(@"drawingsList.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, outputDrawingsList);
                };

                foreach(var (d, i) in outputDrawingsList.Select((v, i) => (v, i)))
                    LogTrace("Drawing {0}: {1}", i, d);
            }
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