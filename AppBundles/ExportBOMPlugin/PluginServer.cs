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
using System.Reflection;
using System.Runtime.InteropServices;
using Inventor;

namespace ExportBOMPlugin
{
    [Guid("282d00a7-b2b5-488e-8a76-7c22280794a6")]
    public class PluginServer : ApplicationAddInServer
    {
        // Inventor application object.
        private InventorServer _inventorServer;

        public dynamic Automation { get; private set; }

        public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            Trace.TraceInformation(": ExportBOMPlugin (" + Assembly.GetExecutingAssembly().GetName().Version.ToString(4) + "): initializing... ");

            // Initialize AddIn members.
            _inventorServer = addInSiteObject.InventorServer;
            Automation = new ExportBOMAutomation(_inventorServer);
        }

        public void Deactivate()
        {
            Trace.TraceInformation(": ExportBOMPlugin: deactivating... ");

            // Release objects.
            Marshal.ReleaseComObject(_inventorServer);
            _inventorServer = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int CommandID)
        {
            // obsolete
        }
    }
}
