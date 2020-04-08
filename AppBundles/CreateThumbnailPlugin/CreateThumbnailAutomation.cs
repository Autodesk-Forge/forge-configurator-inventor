using System;
using System.Diagnostics;
using System.IO;
using Path = System.IO.Path;
using System.Runtime.InteropServices;
using Inventor;

namespace CreateThumbnailPlugin
{
    [ComVisible(true)]
    public class CreateThumbnailAutomation
    {
        private readonly InventorServer inventorApplication;

        public CreateThumbnailAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            Trace.TraceInformation("Run called with {0}", doc.DisplayName);
            RunWithArguments(doc, null);
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            Trace.TraceInformation("Processing " + doc.FullFileName);
            dynamic invDoc = doc;
            string thumbnailsFolder = Path.Combine(Directory.GetCurrentDirectory(), "ThumbnailImages");
            string fileName = "thumbnail.png";
            string filePath = System.IO.Path.Combine(thumbnailsFolder, fileName);
            Camera cam = inventorApplication.TransientObjects.CreateCamera();
            cam.SceneObject = invDoc.ComponentDefinition;
            cam.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
            cam.Fit();
            cam.ApplyWithoutTransition();
            cam.SaveAsBitmap(filePath, 30, 30, Type.Missing, Type.Missing);
            Trace.TraceInformation($"Saved thumbnail as {filePath}");
        }
    }
}