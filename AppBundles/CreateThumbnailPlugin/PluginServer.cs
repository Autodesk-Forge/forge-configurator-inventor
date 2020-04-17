using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Inventor;

namespace CreateThumbnailPlugin
{
    [Guid("9779EFBE-3CA7-45A6-AE90-DA85485DD674")]
    public class PluginServer : ApplicationAddInServer
    {
        // Inventor application object.
        private InventorServer _inventorServer;
        public dynamic Automation { get; private set; }

        public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            Trace.TraceInformation(": CreateThumbnailPlugin (" + Assembly.GetExecutingAssembly().GetName().Version.ToString(4) + "): initializing...");
            // Initialize AddIn members.
            _inventorServer = addInSiteObject.InventorServer;
            Automation = new CreateThumbnailAutomation(_inventorServer);
        }

        public void Deactivate()
        {
            Trace.TraceInformation(": CreateThumbnailPlugin: deactivating... ");
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