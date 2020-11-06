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

using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils.Helpers;

namespace Shared
{
    [ComVisible(true)]
    public abstract class AutomationBase
    {
        protected readonly InventorServer _inventorApplication;

        public AutomationBase(InventorServer inventorApp)
        {
            _inventorApplication = inventorApp;
        }

        public virtual void Run(Document doc)
        {
            LogTrace($"Run called with {doc.DisplayName}");
            LogError("Input arguments are expected. The processing failed.");
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            if (doc == null && map.HasKey("ilod"))
            {
                string docPath = map.AsString("ilod");
                LogTrace($"Opening /ilod document name: {docPath}");

                if (docPath.ToUpper().EndsWith(".IAM"))
                {
                    FileManager fm = _inventorApplication.FileManager;

                    string dvActRep = fm.GetLastActiveDesignViewRepresentation(docPath);
                    LogTrace($"LastActiveDesignViewRepresentation: {dvActRep}");

                    string lodActRep = fm.GetLastActiveLevelOfDetailRepresentation(docPath);
                    LogTrace($"LastActiveLevelOfDetailRepresentation: {lodActRep}");

                    NameValueMap openOptions = _inventorApplication.TransientObjects.CreateNameValueMap();
                    openOptions.Add("LevelOfDetailRepresentation", lodActRep);
                    openOptions.Add("DesignViewRepresentation", dvActRep);

                    doc = _inventorApplication.Documents.OpenWithOptions(docPath, openOptions, false);
                }
                else
                {
                    doc = _inventorApplication.Documents.Open(docPath, false);
                }
                LogTrace($"Full document name: {doc.FullDocumentName}");
            }

            ExecWithArguments(doc, map);
        }

        public abstract void ExecWithArguments(Document doc, NameValueMap map);

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
            protected static void LogTrace(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        protected static void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        protected static void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        protected static void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}