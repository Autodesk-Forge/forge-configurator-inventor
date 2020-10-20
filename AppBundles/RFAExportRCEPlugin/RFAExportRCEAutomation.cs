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

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;

namespace RFAExportRCEPlugin
{
    [ComVisible(true)]
    public class RFAExportRCEAutomation
    {
        private readonly InventorServer inventorApplication;

        public RFAExportRCEAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace("Run called with {0}", doc.DisplayName);
            try
            {
                using (new HeartBeat())
                {
                    ExportRFA(doc);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {

        }

        BIMComponent getBIMComponent(Document doc)
        {
            BIMComponent bimComponent = null;
            var docType = doc.DocumentType;
            if (docType == DocumentTypeEnum.kAssemblyDocumentObject)
            {
                AssemblyDocument _doc = doc as AssemblyDocument;
                bimComponent = _doc.ComponentDefinition.BIMComponent;
            }
            else if (docType == DocumentTypeEnum.kPartDocumentObject)
            {
                PartDocument _doc = doc as PartDocument;
                bimComponent = _doc.ComponentDefinition.BIMComponent;
            }
            else
            {
                Trace.TraceInformation("NOT supported document type.");
            }

            return bimComponent;
        }

        #region ExportRFA file 

        public void ExportRFA(Document doc) 
        { 
            LogTrace("Export RFA file.");

            BIMComponent bimComponent = getBIMComponent(doc);
            if (bimComponent == null)
            {
                return;
            }

            var startDir = System.IO.Directory.GetCurrentDirectory();

            // output file name
            var fileName = System.IO.Path.Combine(startDir, "Output.rfa");

            NameValueMap nvm = inventorApplication.TransientObjects.CreateNameValueMap();
            LogTrace($"Exporting to {fileName}");

            var reportFileName = System.IO.Path.Combine(startDir, "Report.html");

            LogTrace($"Reporting to {reportFileName}");
            nvm.Add("ReportFileName", reportFileName);

            DateTime t = DateTime.Now;
            using (new HeartBeat())
            {
                try
                {
                    bimComponent.ExportBuildingComponentWithOptions(fileName, nvm);
                    LogTrace("Exporting finished");
                    LogTrace($"EXPORT took : {(DateTime.Now - t).TotalSeconds} seconds");
                }
                catch (Exception e)
                {
                    LogTrace($"Exporting FAILED : {e.Message}");
                }
            }

            if (System.IO.File.Exists(fileName))
            {
                LogTrace($"EXPORTED to : {fileName}");
            }
            else
            {
                LogTrace($"EXPORT does not exist !!!!!!!");
            }

            if (System.IO.File.Exists(reportFileName))
            {
                LogTrace($"REPORT generated.");
            }
            else
            {
                LogTrace($"REPORT does not exist !!!!!!!");
            }

        }
        #endregion

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