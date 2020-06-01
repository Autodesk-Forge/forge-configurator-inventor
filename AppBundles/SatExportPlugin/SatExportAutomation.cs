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

namespace SatExportPlugin
{
    [ComVisible(true)]
    public class SatExportAutomation
    {
        private readonly InventorServer inventorApplication;

        public SatExportAutomation(InventorServer inventorApp)
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
                    ExportSAT(doc);
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
        
        #region ExportSAT file 

        public void ExportSAT(Document doc) 
        { 
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();

            LogTrace("Export SAT file.");
            TranslatorAddIn SAT_AddIn = (TranslatorAddIn)inventorApplication.ApplicationAddIns.ItemById["{89162634-02B6-11D5-8E80-0010B541CD80}"];

            if (SAT_AddIn == null)
            {
                LogTrace("Could not access to SAT translator ...");
                return;
            }

            TranslationContext oContext = inventorApplication.TransientObjects.CreateTranslationContext();
            NameValueMap map = inventorApplication.TransientObjects.CreateNameValueMap();

            if (SAT_AddIn.get_HasSaveCopyAsOptions(doc, oContext, map))
            {
                LogTrace("SAT: Set context type");
                oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism;

                LogTrace("SAT: create data medium");
                DataMedium oData = inventorApplication.TransientObjects.CreateDataMedium();

                LogTrace("SAT save to: " + currentDirectory + "\\export.sat");
                oData.FileName = currentDirectory + "\\export.sat";

                map.set_Value("GeometryType", 1);

                SAT_AddIn.SaveCopyAs(doc, oContext, map, oData);
                LogTrace("SAT exported.");
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