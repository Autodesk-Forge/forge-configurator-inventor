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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Inventor;
using Newtonsoft.Json;
using File = System.IO.File;
using Path = System.IO.Path;

namespace DrawingsListPlugin
{
    public enum Severity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public class Message
    {
        public string Text { get; set; }
        public Severity Severity { get; set; }
    }

    [ComVisible(true)]
    public class DrawingsListAutomation
    {
        private readonly InventorServer inventorApplication;
        private readonly List<Message> _messages = new List<Message>();

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
            // mask to exclude Inventor backup dirs

            using (new HeartBeat())
            {
                ExtractDrawingList();
                SaveMessages();
            }
        }

        private void ExtractDrawingList()
        {
            const string oldVersionMask = @"oldversions\";
            var drawingExtensions = new List<string> {".idw", ".dwg"};

            var rootDir = Directory.GetCurrentDirectory();
            var index = Path.Combine(rootDir, "unzippedIam").Length + 1;

            var drawings = Directory.GetFiles(rootDir, "*.*", SearchOption.AllDirectories)
                .Where(file => drawingExtensions.IndexOf(Path.GetExtension(file.ToLower())) >= 0 &&
                               !file.ToLower().Contains(oldVersionMask))
                .Select(path => path.Substring(index))
                .ToArray();

            SaveAsJson(drawings, "drawingsList.json");

            AddMessage($"Found {drawings.Length} drawings", Severity.Info);
        }

        private void SaveMessages()
        {
            SaveAsJson(_messages, "messages.json");
        }

        private void AddMessage(string message, Severity severity)
        {
            _messages.Add(new Message { Text = message, Severity = severity });
        }

        /// <summary>
        /// Serialize data to JSON file.
        /// </summary>
        private void SaveAsJson<T>(T data, string fileName)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, data);
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