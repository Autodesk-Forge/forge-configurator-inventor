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
using System.Collections.Generic;
using WebApplication.State;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Names for local files.
    /// </summary>
    internal static class LocalName
    {
        public const string SvfDir = "SVF";

        /// <summary>
        /// Project metadata.
        /// </summary>
        public const string Metadata = "metadata.json";

        /// <summary>
        /// Thumbnail.
        /// </summary>
        public const string Thumbnail = "thumbnail.png";

        /// <summary>
        /// Names of drawings in the project.
        /// </summary>
        public const string DrawingsList = "drawingsList.json";

        /// <summary>
        /// User-oriented messages after adoption.
        /// </summary>
        public const string AdoptMessages = "adopt-messages.json";

        /// <summary>
        /// ZIP archive with SVF model.
        /// </summary>
        public const string ModelView = "model-view.zip";

        /// <summary>
        /// JSON file with parameters.
        /// </summary>
        public const string Parameters = "parameters.json";

        /// <summary>
        /// JSON file with BOM data.
        /// </summary>
        public const string BOM = "bom.json";
        
        /// <summary>
        /// Drawing in format ForgeView can load and show
        /// </summary>
        public static string DrawingPdf(int index) => $"drawing_{index}.pdf";

        /// <summary>
        /// Files with statistics.
        /// </summary>
        public static class Stats
        {
            public const string Adopt = "stats.adopt.json";
            public const string Update = "stats.update.json";
            public const string RFA = "stats.rfa.json";
            public static string DrawingPDF(int index) => $"stats.drawing_{index}.pdf.json";
            public const string Drawings = "stats.drawing.zip.json";
        }
    }

    /// <summary>
    /// Object Name Constants
    /// </summary>
    public static class ONC // aka ObjectNameConstants
    {
        /// <summary>
        /// Separator to fake directories in OSS filename.
        /// </summary>
        private const string OssSeparator = "/"; // This must stay private

        public const string ProjectsFolder = "projects";
        public const string ShowParametersChanged = "showparameterschanged.json";
        public const string CacheFolder = "cache";
        public const string AttributesFolder = "attributes";


        public static readonly string ProjectsMask = ToPathMask(ProjectsFolder);

        public static string ProjectUrl(string projectName)
        {
            return Join(ProjectsFolder, projectName);
        }

        /// <summary>
        /// Extract project name from OSS object name.
        /// </summary>
        /// <param name="ossObjectName">OSS name for the project</param>
        public static string ToProjectName(string ossObjectName)
        {
            if(!ossObjectName.StartsWith(ProjectsMask))
            {
                throw new ApplicationException("Initializing Project from invalid bucket key: " + ossObjectName);
            }

            return ossObjectName.Substring(ProjectsMask.Length);
        }

        /// <summary>
        /// Get collection of OSS search masks for the project.
        /// It allows to get all OSS files related to the project.
        /// </summary>
        /// <remarks>
        /// The collection does NOT include name of the project itself. Use <see cref="Project.ExactOssName"/> to generate it.
        /// </remarks>
        public static IEnumerable<string> ProjectFileMasks(string projectName)
        {
            yield return ToPathMask(AttributesFolder, projectName);
            yield return ToPathMask(CacheFolder, projectName);
        }

        /// <summary>
        /// Join path pieces into OSS name.
        /// </summary>
        public static string Join(params string[] pieces)
        {
            return string.Join(OssSeparator, pieces);
        }

        /// <summary>
        /// Generate OSS name, which serves as a folder mask for subfolders and files.
        /// </summary>
        private static string ToPathMask(params string[] pieces)
        {
            return Join(pieces) + OssSeparator;
        }
    }

    /// <summary>
    /// OSS does not support directories, so emulate folders with long file names.
    /// </summary>
    public class OssNameConverter
    {
        private readonly string _namePrefix;

        public OssNameConverter(string namePrefix)
        {
            _namePrefix = namePrefix;
        }

        /// <summary>
        /// Generate full OSS name for the filename.
        /// </summary>
        public string ToFullName(string fileName)
        {
            return ONC.Join(_namePrefix, fileName);
        }

        /// <summary>
        /// Shorthand for <see cref="ToFullName"/>.
        /// </summary>
        public string this[string fileName] => ToFullName(fileName);
    }

    /// <summary>
    /// Project owned filenames under "parameters hash" directory at OSS.
    /// </summary>
    public class OSSObjectNameProvider : OssNameConverter
    {
        public OSSObjectNameProvider(string projectName, string parametersHash) :
                base(ONC.Join(ONC.CacheFolder, projectName, parametersHash)) {}

        /// <summary>
        /// Filename for ZIP with current model state.
        /// </summary>
        public string GetCurrentModel(bool isAssembly)
        {
            return ToFullName(isAssembly ? "model.zip" : "model.ipt");
        }

        /// <summary>
        /// Filename for ZIP with SVF model.
        /// </summary>
        public string ModelView => ToFullName(LocalName.ModelView);

        /// <summary>
        /// Filename for JSON with Inventor document parameters.
        /// </summary>
        public string Parameters => ToFullName(LocalName.Parameters);

        public string Rfa => ToFullName("result.rfa");

        /// <summary>
        /// Filename for JSON with BOM data.
        /// </summary>
        public string Bom => ToFullName(LocalName.BOM);

        /// <summary>
        /// Filename for JSON with BOM data.
        /// </summary>
        public string DrawingsList => ToFullName(LocalName.DrawingsList);

        public string Drawing => ToFullName("drawing.zip");

        /// <summary>
        /// Filename for PDF with drawing.
        /// </summary>
        public string DrawingPdf(int idx) => ToFullName(LocalName.DrawingPdf(idx));

        public string StatsAdopt => ToFullName(LocalName.Stats.Adopt);
        public string StatsUpdate => ToFullName(LocalName.Stats.Update);
        public string StatsRFA => ToFullName(LocalName.Stats.RFA);
        public string StatsDrawingPDF(int index) => ToFullName(LocalName.Stats.DrawingPDF(index));
        public string StatsDrawings => ToFullName(LocalName.Stats.Drawings);
    }

    /// <summary>
    /// Project owned filenames in Attributes directory at OSS.
    /// </summary>
    public class OssAttributes : OssNameConverter
    {
        /// <summary>
        /// Filename for thumbnail image.`
        /// </summary>
        public string Thumbnail => ToFullName(LocalName.Thumbnail);

        /// <summary>
        /// Filename of JSON file with project metadata.
        /// </summary>
        public string Metadata => ToFullName(LocalName.Metadata);

        public string DrawingsList => ToFullName(LocalName.DrawingsList);
        public string AdoptMessages => ToFullName(LocalName.AdoptMessages);

        /// <summary>
        /// Constructor.
        /// </summary>
        public OssAttributes(string projectName) : base(ONC.Join(ONC.AttributesFolder, projectName)) {}
    }
}
