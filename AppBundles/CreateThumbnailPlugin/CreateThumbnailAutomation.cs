using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Path = System.IO.Path;
using System.Runtime.InteropServices;
using Inventor;
using Color = Inventor.Color;
using File = Inventor.File;

namespace CreateThumbnailPlugin
{
    [ComVisible(true)]
    public class CreateThumbnailAutomation
    {
        private const int ThumbnailSize = 30;
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

                // TODO: only IAM and IPT are supported now, but it's not validated
                invDoc.ObjectVisibility.AllWorkFeatures = false;
                invDoc.ObjectVisibility.Sketches = false;
                invDoc.ObjectVisibility.Sketches3D = false;
                invDoc.ObjectVisibility.WeldmentSymbols = false;

                string fileNameLarge = "thumbnail-large.png";
                string filePathLarge = Path.Combine(Directory.GetCurrentDirectory(), fileNameLarge);


                inventorApplication.DisplayOptions.Show3DIndicator = false;
                Camera cam = inventorApplication.TransientObjects.CreateCamera();
                cam.SceneObject = invDoc.ComponentDefinition;
                cam.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
                cam.Fit();
                cam.ApplyWithoutTransition();

                Color backgroundColor = inventorApplication.TransientObjects.CreateColor(0xEC, 0xEC, 0xEC, 0.0); // hardcoded. Make as a parameter
                cam.SaveAsBitmap(filePathLarge, ThumbnailSize * 2, ThumbnailSize * 2, backgroundColor, backgroundColor);

                using (var image = Image.FromFile(filePathLarge))
                using (var destImage = new Bitmap(ThumbnailSize, ThumbnailSize))
                {
                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            var destRect = new Rectangle(0, 0, ThumbnailSize, ThumbnailSize);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    string fileName = "thumbnail.png";
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                    destImage.Save(filePath);

                    LogTrace($"Saved thumbnail as {filePath}");
                }

                System.IO.File.Delete(filePathLarge);
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
