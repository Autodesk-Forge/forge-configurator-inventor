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
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Inventor;
using PluginUtilities;

namespace ExtractParametersPlugin
{
    [ComVisible(true)]
    public class ExtractParametersAutomation
    {
        private readonly InventorServer _inventorApplication;

        public ExtractParametersAutomation(InventorServer inventorApp)
        {
            _inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace($"Run called with {doc.DisplayName}");

            try
            {
                using (new HeartBeat())
                {
                    Parameters parameters;
                    switch (doc.DocumentType)
                    {
                        case DocumentTypeEnum.kPartDocumentObject:
                            parameters = ((PartDocument) doc).ComponentDefinition.Parameters;
                            break;
                        case DocumentTypeEnum.kAssemblyDocumentObject:
                            parameters = ((AssemblyDocument) doc).ComponentDefinition.Parameters;
                            break;
                        default:
                            LogError($"Unsupported document type: {doc.DocumentType}");
                            return;
                    }

                    var paramsExtractor = new ParametersExtractor();
                    paramsExtractor.Extract(doc, parameters);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

 

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        public void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        public void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}