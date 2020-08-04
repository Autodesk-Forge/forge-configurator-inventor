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

namespace ExportBOMPlugin
{
    public class ExtractedRow
    {
        public string RowNumber { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Material { get; set; }
    }

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

        private readonly InventorServer inventorApplication;

        public ExportBOMAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace($"Run called with {doc.DisplayName}");
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            LogTrace("Processing " + doc.FullFileName);

            try
            {
                string bomJson;
                switch (doc.DocumentType)
                {
                    case DocumentTypeEnum.kPartDocumentObject:
                        LogTrace("No BOM for Part documents.");
                        bomJson = "[]"; // empty JSON array
                        break;

                    case DocumentTypeEnum.kAssemblyDocumentObject:

                        List<ExtractedRow> rows = ProcessAssembly((AssemblyDocument)doc);
                        bomJson = JsonConvert.SerializeObject(rows, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None });
                        break;

                    // complain about non-supported document types
                    default:
                        throw new ArgumentOutOfRangeException(nameof(doc), "Unsupported document type");
                }

                System.IO.File.WriteAllText(OutputJsonName, bomJson);
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        private List<ExtractedRow> ProcessAssembly(AssemblyDocument doc)
        {
            using (new HeartBeat())
            {
                var rows = new List<ExtractedRow>();

                AssemblyComponentDefinition assemblyComponentDef = doc.ComponentDefinition;
                BOM bom = assemblyComponentDef.BOM;
                BOMViews bomViews = bom.BOMViews;
                BOMView structureView = bomViews["Structured"];

                GetBomRowProperties(structureView.BOMRows, rows);

                return rows;
            }
        }

        private void GetBomRowProperties(BOMRowsEnumerator bomRowsEnumerator, List<ExtractedRow> rows)
        {
            foreach (BOMRow row in bomRowsEnumerator)
            {
                ComponentDefinition componentDef = row.ComponentDefinitions[1];
                var trackingSet = componentDef.Document.PropertySets[TrackingProperties];

                // Assumes not virtual component (if so add conditional for that here)
                Property partNum = trackingSet["Part Number"];
                Property description = trackingSet["Description"];
                Property material = trackingSet["Material"];

                var extracted = new ExtractedRow
                                    {
                                        RowNumber = row.ItemNumber,
                                        PartNumber = partNum.Value,
                                        Quantity = row.ItemQuantity,
                                        Description = description.Value,
                                        Material = material.Value
                                    };

                rows.Add(extracted);

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