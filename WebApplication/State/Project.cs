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
using WebApplication.Utilities;

namespace WebApplication.State
{
    public class Project
    {
        public Project(string projectName, string rootDir)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentException("Initializing Project with empty name", nameof(projectName));
            }

            Name = projectName; 
            OSSSourceModel = ExactOssName(projectName);
            OSSSourceDrawings = ExactOssName(projectName, "Drawings");

            OssAttributes = new OssAttributes(projectName);
            LocalAttributes = new LocalAttributes(rootDir, Name);
        }

        public static string ExactOssName(string projectName, string suffix="") => string.IsNullOrEmpty(suffix) ? ONC.Join(ONC.ProjectsFolder, projectName) : ONC.Join(ONC.ProjectsFolder, projectName, suffix);

        public string? Name { get; }
        public string? OSSSourceModel { get; }
        public string? OSSSourceDrawings { get; }

        public OSSObjectNameProvider OssNameProvider(string? hash) => new (Name, hash);
        public LocalNameProvider LocalNameProvider(string? hash) => new (LocalAttributes.BaseDir, hash!);

        /// <summary>
        /// Full local names for project attribute files.
        /// </summary>
        public LocalAttributes LocalAttributes { get; }

        /// <summary>
        /// Full names for project attributes files (metadata, thumbnails, etc.) at OSS.
        /// </summary>
        public OssAttributes OssAttributes { get; }
    }
}
