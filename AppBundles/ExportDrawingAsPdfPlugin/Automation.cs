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

using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Shared;

namespace ExportDrawingAsPdfPlugin
{
    [ComVisible(true)]
    public class Automation : AutomationBase
    {
        public Automation(InventorServer inventorApp) : base(inventorApp)
        {
        }

        public override void ExecWithArguments(Document doc, NameValueMap map)
        {
            using (new HeartBeat())
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                var drawingToGenerate = map.Item["_2"];

                if (drawingToGenerate == null)
                {
                    LogTrace("Drawing not specified !");
                }

                LogTrace("Drawing to generate PDF: {0}", drawingToGenerate);

                NameValueMap openOptions = _inventorApplication.TransientObjects.CreateNameValueMap();

                string drawing = System.IO.Path.Combine(dir, /* ??? */"unzippedIam", drawingToGenerate);
                LogTrace("Exporting : " + drawing);
                var drawingDocument = _inventorApplication.Documents.OpenWithOptions(drawing, openOptions, true);
                LogTrace("Drawing opened");
                drawingDocument.Update2(true);
                LogTrace("Drawing updated");
                drawingDocument.Save2(true);
                LogTrace("Drawing saved");
                var pdfPath = System.IO.Path.Combine(dir, "Drawing.pdf");
                LogTrace("Exporting drawing to: " + pdfPath);
                ExportIDWToPDF(drawingDocument, pdfPath);
                LogTrace("Drawing exported");
            }
        }

        private void ActivateDefaultProject(string dir)
        {
            var defaultProjectName = "FDADefault";
            
            var projectFullFileName = System.IO.Path.Combine(dir, defaultProjectName + ".ipj");

            DesignProject project = null;
            if (System.IO.File.Exists(projectFullFileName))
            {
                project = _inventorApplication.DesignProjectManager.DesignProjects.AddExisting(projectFullFileName);
                Trace.TraceInformation("Adding existing default project file: {0}", projectFullFileName);

            } else
            {
                project = _inventorApplication.DesignProjectManager.DesignProjects.Add(MultiUserModeEnum.kSingleUserMode, defaultProjectName, dir);
                Trace.TraceInformation("Creating default project file with name: {0} at {1}", defaultProjectName, dir);
            }
            
            Trace.TraceInformation("Activating default project {0}", project.FullFileName);
            project.Activate(true);
        }

        // Export Drawing file to PDF format
        // In the case that the Drawing has more sheets -> it will export PDF with pages
        // Each PDF page represents one Drawing sheet
        public void ExportIDWToPDF(Document doc, string exportFileName)
        {
            if (doc == null)
            {
                LogError("Document is null!");
                return;
            }

            LogTrace("PDF file full path : " + exportFileName);

            LogTrace("Create PDF Translator Addin");
            TranslatorAddIn PDFAddIn = _inventorApplication.ApplicationAddIns.ItemById["{0AC6FD96-2F4D-42CE-8BE0-8AEA580399E4}"] as TranslatorAddIn;

            if (PDFAddIn == null)
            {
                LogError("Error: PDF Translator Addin is null!");
                return;
            }

            TranslationContext context = _inventorApplication.TransientObjects.CreateTranslationContext();
            NameValueMap options = _inventorApplication.TransientObjects.CreateNameValueMap();
            if (PDFAddIn.HasSaveCopyAsOptions[doc, context, options])
            {
                context.Type = IOMechanismEnum.kFileBrowseIOMechanism;
                DataMedium dataMedium = _inventorApplication.TransientObjects.CreateDataMedium();

                options.Value["Sheet_Range"] = PrintRangeEnum.kPrintAllSheets;
                options.Value["Vector_Resolution"] = 300;

                options.Value["All_Color_AS_Black"] = false;
                options.Value["Sheets"] = GetSheetOptions(doc);

                dataMedium.FileName = exportFileName;
                LogTrace("Processing PDF export ...");
                PDFAddIn.SaveCopyAs(doc, context, options, dataMedium);
                LogTrace("Finish processing PDF export ...");
            }
        }

        // Check if the Drawing file has more sheets
        private NameValueMap GetSheetOptions(Document doc)
        {
            DrawingDocument drawingDocument = doc as DrawingDocument;

            NameValueMap sheets = _inventorApplication.TransientObjects.CreateNameValueMap();
            Debug.Assert(drawingDocument != null, nameof(drawingDocument) + " != null");
            foreach (Sheet sheet in drawingDocument.Sheets)
            {
                NameValueMap option = _inventorApplication.TransientObjects.CreateNameValueMap();
                option.Add("Name", sheet.Name);
                option.Add("3DModel", false);
                sheets.Add("Sheet" + sheets.Count + 1, option);
            }

            return sheets;
        }
    }
}