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
            try
            {
                LogTrace("Processing " + doc.FullFileName);
                dynamic invDoc = doc;
                string fileName = "thumbnail.png";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                inventorApplication.DisplayOptions.Show3DIndicator = false;
                Camera cam = inventorApplication.TransientObjects.CreateCamera();
                cam.SceneObject = invDoc.ComponentDefinition;
                cam.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
                cam.Fit();
                cam.ApplyWithoutTransition();
                cam.SaveAsBitmap(filePath, 30, 30, Type.Missing, Type.Missing);
                LogTrace($"Saved thumbnail as {filePath}");
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
