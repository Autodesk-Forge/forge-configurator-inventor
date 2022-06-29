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
using Shared;

namespace RFAExportRCEPlugin
{
    [ComVisible(true)]
    public class RFAExportRCEAutomation : AutomationBase
    {
        public RFAExportRCEAutomation(InventorServer inventorApp) : base(inventorApp)
        {
        }

        public override void ExecWithArguments(Document doc, NameValueMap map)
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

            NameValueMap nvm = _inventorApplication.TransientObjects.CreateNameValueMap();
            LogTrace($"Exporting to {fileName}");

            var reportFileName = System.IO.Path.Combine(startDir, "Report.html");

            LogTrace($"Reporting to {reportFileName}");
            nvm.Add("ReportFileName", reportFileName);

            DateTime t = DateTime.Now;
            DateTime t2;
            using (new HeartBeat())
            {
                try
                {
                    bimComponent.ExportBuildingComponentWithOptions(fileName, nvm);
                    LogTrace("Export finished");
                    t2 = DateTime.Now;
                }
                catch (Exception e)
                {
                    t2 = DateTime.Now;
                    LogTrace($"ERROR: Exporting FAILED : {e.Message}");
                }
            }

            if (System.IO.File.Exists(fileName))
            {
                LogTrace($"EXPORTED to : {fileName}");
                LogTrace($"EXPORT took : {(t2 - t).TotalSeconds} seconds");
            }
            else
            {
                LogTrace("ERROR: EXPORT does not exist !!!!!!!");
            }

            LogTrace(System.IO.File.Exists(reportFileName)
                ? "REPORT generated."
                : "ERROR: REPORT does not exist !!!!!!!");
        }
        #endregion
    }
}