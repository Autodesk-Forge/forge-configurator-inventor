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
using System.Linq;
using System.Runtime.InteropServices;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Inventor;
using Newtonsoft.Json;
using Shared;
using File = System.IO.File;
using Path = System.IO.Path;

namespace DataCheckerPlugin
{
    [ComVisible(true)]
    public class DataCheckerAutomation
    {
        private readonly InventorServer inventorApplication;
        private readonly List<Message> _messages = new List<Message>();

        private readonly HashSet<string> _interests = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> _notSupported = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                    {
                                                                       { "{AC211AE0-A7A5-4589-916D-81C529DA6D17}", "Frame Generator" },
                                                                       { "{4D39D5F1-0985-4783-AA5A-FC16C288418C}", "Tube & Pipe" },
                                                                       { "{C6107C9D-C53F-4323-8768-F65F857F9F5A}", "Cable & Harness" },
                                                                       { "{24E39891-3782-448F-8C33-0D8D137148AC}", "Mold Design" },
                                                                       { "{3D88D7B5-6DD8-4205-A2B5-2B51F7BF74A7}", "Content Center" },
                                                                       { "{BB8FE430-83BF-418D-8DF9-9B323D3DB9B9}", "Design Accelerator" },
                                                                    };

        public DataCheckerAutomation(InventorServer inventorApp)
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
                ExtractDrawingList();
                DetectUnsupportedAddins(doc);

                SaveMessages();
            }
        }

        private void DetectUnsupportedAddins(Document doc)
        {
            LogTrace("Detecting unsupported addins");

            // collect interests for the hierarchy
            GetInterests(doc);
            foreach (Document d in doc.AllReferencedDocuments)
            {
                GetInterests(d);
            }

            // check if non-supported addins are in the list
            var addinNames = new List<string>();
            foreach (var addinId in _interests)
            {
                if (_notSupported.TryGetValue(addinId, out string name))
                {
                    addinNames.Add(name);
                }
            }

            // log message (if necessary)
            switch (addinNames.Count)
            {
                case 1:
                    AddMessage($"Detected unsupported plugin: {addinNames[0]}", Severity.Error);
                    break;
                case 0:
                    break;
                default:
                    AddMessage($"Detected unsupported plugins: {string.Join(", ", addinNames)}", Severity.Error);
                    break;
            }
        }

        /// <summary>
        /// Extract interests for the document.
        /// </summary>
        private void GetInterests(Document doc)
        {
            // process only assemblies and parts
            var docType = doc.DocumentType;
            if (docType != DocumentTypeEnum.kAssemblyDocumentObject && docType != DocumentTypeEnum.kPartDocumentObject)
                return;

            foreach (DocumentInterest di in doc.DocumentInterests)
            {
                if (di.InterestType != DocumentInterestTypeEnum.kInterested) continue;

                _interests.Add(di.ClientId);
            }
        }

        /// <summary>
        /// Collect relative paths for drawings.
        /// </summary>
        private void ExtractDrawingList()
        {
            LogTrace("Extracting drawings list");

            // mask to exclude Inventor backup dirs
            const string oldVersionMask = @"oldversions\";
            var drawingExtensions = new List<string> {".idw", ".dwg"};

            var rootDir = Directory.GetCurrentDirectory();
            var index = Path.Combine(rootDir, "unzippedIam").Length + 1;

            var drawings = Directory.GetFiles(rootDir, "*.*", SearchOption.AllDirectories)
                .Where(file =>
                {
                    var lowName = file.ToLower();
                    return drawingExtensions.IndexOf(Path.GetExtension(lowName)) >= 0 &&
                           !lowName.Contains(oldVersionMask);
                })
                .Select(path => path.Substring(index))
                .ToArray();

            SaveAsJson(drawings, "drawings-list.json"); // the file name must be in sync with activity definition

            AddMessage($"Found {drawings.Length} drawings", Severity.Info);
        }

        private void SaveMessages()
        {
            SaveAsJson(_messages, "adopt-messages.json"); // the file name must be in sync with activity definition

            LogTrace("Collected user messages:");
            LogTrace(JsonConvert.SerializeObject(_messages, Formatting.Indented));
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