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
    public class DataCheckerAutomation : AutomationBase
    {
        private readonly List<Message> _messages = new List<Message>();

        private readonly HashSet<string> _interests = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> _notSupported = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                    {
                                                                       { "{AC211AE0-A7A5-4589-916D-81C529DA6D17}", "Frame Generator" },
                                                                       { "{4D39D5F1-0985-4783-AA5A-FC16C288418C}", "Tube & Pipe" },
                                                                       { "{C6107C9D-C53F-4323-8768-F65F857F9F5A}", "Cable & Harness" },
                                                                       { "{24E39891-3782-448F-8C33-0D8D137148AC}", "Mold Design" },
                                                                       { "{BB8FE430-83BF-418D-8DF9-9B323D3DB9B9}", "Design Accelerator" },
                                                                    };

        private readonly HashSet<string> _missingReferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public DataCheckerAutomation(InventorServer inventorApp) : base(inventorApp)
        {
        }

        public override void ExecWithArguments(Document doc, NameValueMap map)
        {
            using (new HeartBeat())
            {
                ExtractDrawingList(doc);
                DetectUnsupportedAddins(doc);
                CheckForMissingReferences(doc);

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
                    AddMessage($"Detected unsupported plugin: {addinNames[0]}.", Severity.Warning);
                    break;
                case 0:
                    break;
                default:
                    AddMessage($"Detected unsupported plugins: {string.Join(", ", addinNames)}.", Severity.Warning);
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

        class DefaultDocComparer : IComparer<string>
        {
            private readonly string _defaultDoc;
            private string _found = null;
            public DefaultDocComparer(string defaultDoc) { _defaultDoc = defaultDoc; }
            public int Compare(string x, string y)
            {
                var filenameX = Path.GetFileNameWithoutExtension(x).ToLower();
                var filenameY = Path.GetFileNameWithoutExtension(y).ToLower();

                if (filenameX.Equals(filenameY))
                    return 0;

                // special handling for default filename (remember the first one)
                if (filenameX == _defaultDoc && (_found == null || _found == x))
                {
                    _found = x;
                    return -1;
                }
                if (filenameY == _defaultDoc && (_found == null || _found == y))
                {
                    _found = y;
                    return 1;
                }

                // default compare
                return x.CompareTo(y);
            }
        }

        /// <summary>
        /// Collect relative paths for drawings.
        /// </summary>
        private void ExtractDrawingList(Document doc)
        {
            LogTrace("Extracting drawings list");

            // mask to exclude Inventor backup dirs
            const string oldVersionMask = @"oldversions\";
            var drawingExtensions = new List<string> {".idw", ".dwg"};

            var rootDir = Directory.GetCurrentDirectory();
            var index = Path.Combine(rootDir, "unzippedIam").Length + 1;
            var fileName = Path.GetFileNameWithoutExtension(doc.FullFileName).ToLower();
            var drawings = Directory.GetFiles(rootDir, "*.*", SearchOption.AllDirectories)
                .Where(file =>
                {
                    var lowName = file.ToLower();
                    return drawingExtensions.IndexOf(Path.GetExtension(lowName)) >= 0 &&
                           !lowName.Contains(oldVersionMask);
                })
                .Select(path => path.Substring(index))
                .OrderBy(path => Path.GetFileName(path), new DefaultDocComparer(fileName))
                .ToArray();

            // drawings is also valid when no drawings exists, so test drawings?.Length
            LogTrace($"DEFAULT drawing is : {(drawings?.Length > 0 ? drawings[0] : "NONE")}");

            SaveAsJson(drawings, "drawings-list.json"); // the file name must be in sync with activity definition

            AddMessage($"Found {drawings.Length} drawings", Severity.Info);

            foreach (var (d, i) in drawings.Select((v, i) => (v, i)))
                LogTrace($"Drawing {i}: {d}");
        }

        private void CheckForMissingReferences(Document doc)
        {
            LogTrace("Scan document for missing references");

            ProcessFileReferences(doc.File);

            if (_missingReferences.Count == 0) return;

            // generate message about files
            var count = _missingReferences.Count;
            var filenames = _missingReferences
                                .Take(2)
                                .ToArray();

            string message;
            switch (count)
            {
                case 1:
                    message = $"Unresolved file: '{filenames[0]}'.";
                    break;
                case 2:
                    message = $"Unresolved files: '{filenames[0]}' and '{filenames[1]}'.";
                    break;
                default: // 3+
                    message = $"Unresolved files: '{filenames[0]}', '{filenames[1]}', and {count - 2} other file(s).";
                    break;
            }

            AddMessage(message, Severity.Warning);
        }

        private void ProcessFileReferences(Inventor.File file)
        {
            foreach (FileDescriptor descriptor in file.ReferencedFileDescriptors)
            {
                if (descriptor.ReferenceMissing)
                {
                    var fileName = Path.GetFileName(descriptor.FullFileName);

                    if (_missingReferences.Contains(fileName)) continue;

                    _missingReferences.Add(fileName);
                    LogError($"Missing '{descriptor.FullFileName}'");
                }
                else if (descriptor.ReferencedFileType != FileTypeEnum.kForeignFileType)
                {
                    // go deeper
                    ProcessFileReferences(descriptor.ReferencedFile);
                }
            }
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
    }
}