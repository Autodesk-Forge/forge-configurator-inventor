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
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared;

namespace ExportBOMPlugin
{
    // heavily based on https://github.com/akenson/da-update-bom
    [ComVisible(true)]
    public class ExportBOMAutomation
    {
        /// <summary>
        /// Filename of output JSON with BOM data.
        /// The filename is expected by corresponding Activity.
        /// </summary>
        private const string OutputJsonName = "bom.json";
        private const string TrackingProperties = "Design Tracking Properties";

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
                                                                                    {
                                                                                        NullValueHandling = NullValueHandling.Ignore,
                                                                                        Formatting = Formatting.None,
                                                                                        ContractResolver = new DefaultContractResolver
                                                                                        {
                                                                                            NamingStrategy = new CamelCaseNamingStrategy()
                                                                                        }
                                                                                    };

        private static readonly ExtractedBOM EmptyBOM = new ExtractedBOM { Columns = new Shared.Column[0], Data = new object[0][] };

        private readonly InventorServer inventorApplication;

        public ExportBOMAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace("Processing " + doc.FullFileName);

            try
            {
                ExtractedBOM extractedBOM = EmptyBOM;

                switch (doc.DocumentType)
                {
                    case DocumentTypeEnum.kPartDocumentObject:
                        LogTrace("No BOM for Part documents.");
                        break;

                    case DocumentTypeEnum.kAssemblyDocumentObject:

                        try
                        {
                            // TODO: remove this project activation when new inventorcoreconsole.exe
                            // will be available on PROD environment
                            var fullFileName = doc.FullFileName;
                            // close the orignal doc
                            doc.Close(true);
                            // activate default project
                            var dir = System.IO.Directory.GetCurrentDirectory();
                            ActivateProject(dir);
                            // open doc with project activated
                            doc = inventorApplication.Documents.Open(fullFileName);
                            // ^

                            extractedBOM = ProcessAssembly((AssemblyDocument)doc);
                        }
                        catch (Exception e)
                        {
                            LogError("Failed to extract BOM. " + e.ToString());
                        }
                        break;

                    // complain about non-supported document types
                    default:
                        throw new ArgumentOutOfRangeException(nameof(doc), "Unsupported document type");
                }

                // save as JSON
                string bomJson = JsonConvert.SerializeObject(extractedBOM, SerializerSettings);
                System.IO.File.WriteAllText(OutputJsonName, bomJson);
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }

        }

        private void ActivateProject(string dir)
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

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            LogError("RunWithArguments is not functional");
        }

        private ExtractedBOM ProcessAssembly(AssemblyDocument doc)
        {
            using (new HeartBeat())
            {
                var extractedBOM = new ExtractedBOM
                                    {
                                        Columns = new[]
                                        {
                                            new Shared.Column { Label = "Row Number" },
                                            new Shared.Column { Label = "Part Number" },
                                            new Shared.Column { Label = "Quantity", Numeric = true },
                                            new Shared.Column { Label = "Description" },
                                            new Shared.Column { Label = "Material" }
                                        },
                                    };
                var rows = new List<object[]>();

                AssemblyComponentDefinition assemblyComponentDef = doc.ComponentDefinition;
                BOM bom = assemblyComponentDef.BOM;
                BOMViews bomViews = bom.BOMViews;
                BOMView structureView = bomViews["Structured"];

                GetBomRowProperties(structureView.BOMRows, rows);

                extractedBOM.Data = rows.ToArray();

                return extractedBOM;
            }
        }

        private void GetBomRowProperties(BOMRowsEnumerator bomRowsEnumerator, List<object[]> rows)
        {
            foreach (BOMRow row in bomRowsEnumerator)
            {
                ComponentDefinition componentDef = row.ComponentDefinitions[1];
                var trackingSet = componentDef.Document.PropertySets[TrackingProperties];

                // Assumes not virtual component (if so add conditional for that here)
                Property partNum = trackingSet["Part Number"];
                Property description = trackingSet["Description"];
                Property material = trackingSet["Material"];

                object[] data = { // order is important. a place to improve
                    row.ItemNumber,
                    partNum.Value,
                    row.ItemQuantity,
                    description.Value,
                    material.Value
                };

                rows.Add(data);

                // iterate through child rows
                if (row.ChildRows != null)
                {
                    GetBomRowProperties(row.ChildRows, rows);
                }
            }
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