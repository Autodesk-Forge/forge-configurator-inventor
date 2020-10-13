﻿/////////////////////////////////////////////////////////////////////
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
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.State
{
    /// <summary>
    /// Logic related to local cache storage for project.
    /// </summary>
    public class ProjectStorage
    {
        /// <summary>
        /// Project names.
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// Project metadata.
        /// </summary>
        public ProjectMetadata Metadata => _lazyMetadata.Value;
        private readonly Lazy<ProjectMetadata> _lazyMetadata;
        public bool IsAssembly => Metadata.IsAssembly;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProjectStorage(Project project)
        {
            Project = project;

            _lazyMetadata = new Lazy<ProjectMetadata>(() =>
                                                            {
                                                                var metadataFile = Project.LocalAttributes.Metadata;
                                                                if (!File.Exists(metadataFile))
                                                                    throw new ApplicationException("Attempt to work with uninitialized project storage.");

                                                                return Json.DeserializeFile<ProjectMetadata>(metadataFile);
                                                            });
        }

        /// <summary>
        /// Ensure the project is cached locally.
        /// </summary>
        public async Task EnsureLocalAsync(OssBucket ossBucket)
        {
            // make sure that there are no leftovers from previous incarnations of the project
            DeleteLocal();

            await EnsureAttributesAsync(ossBucket, ensureDir: true);


            // download ZIP with SVF model
            // NOTE: this step is impossible without having project metadata,
            // because file/dir names depends on hash of initial project state

            await EnsureViewablesAsync(ossBucket, Metadata.Hash, ensureDir: true);
        }

        /// <summary>
        /// Ensure that project attributes are cached locally.
        /// </summary>
        public async Task EnsureAttributesAsync(OssBucket ossBucket, bool ensureDir = true)
        {
            // ensure the directory exists
            var localAttributes = Project.LocalAttributes;
            if (ensureDir)
            {
                Directory.CreateDirectory(localAttributes.BaseDir);
            }

            // download metadata and thumbnail
            var ossAttributes = Project.OssAttributes;
            await Task.WhenAll(
                ossBucket.EnsureFileAsync(ossAttributes.Metadata, localAttributes.Metadata),
                ossBucket.EnsureFileAsync(ossAttributes.Thumbnail, localAttributes.Thumbnail),
                ossBucket.EnsureFileAsync(ossAttributes.AdoptMessages, localAttributes.AdoptMessages),
                ossBucket.EnsureFileAsync(ossAttributes.DrawingsList, localAttributes.DrawingsList)
            );
        }

        /// <summary>
        /// Check if directory for the hash is exists.
        /// It's considered as a sign that project-related data for the hash is cached.
        /// </summary>
        public bool IsCached(string hash)
        {
            var localNames = GetLocalNames(hash);
            return Directory.Exists(localNames.BaseDir);
        }

        /// <summary>
        /// Ensure the project viewables are cached locally.
        /// </summary>
        /// <param name="ossBucket">OSS bucket.</param>
        /// <param name="hash">Parameters hash.</param>
        /// <param name="ensureDir">Create local dir if necessary.</param>
        public async Task EnsureViewablesAsync(OssBucket ossBucket, string hash, bool ensureDir = true)
        {
            var localNames = GetLocalNames(hash);

            // create the "hashed" dir
            if (ensureDir)
            {
                Directory.CreateDirectory(localNames.BaseDir);
            }

            OSSObjectNameProvider ossNames = GetOssNames(hash);
            using var tempFile = new TempFile();
            await Task.WhenAll(
                ossBucket.DownloadFileAsync(ossNames.ModelView, tempFile.Name),
                ossBucket.DownloadFileAsync(ossNames.Parameters, localNames.Parameters),
                ossBucket.DownloadFileAsync(ossNames.Bom, localNames.BOM)
            );

            // extract SVF from the archive
            ZipFile.ExtractToDirectory(tempFile.Name, localNames.SvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported
        }

        public async Task EnsureSvfAsync(OssBucket ossBucket, string hash)
        {
            var localNames = GetLocalNames(hash);
            var ossNames = GetOssNames(hash);

            using var tempFile = new TempFile();
            await ossBucket.DownloadFileAsync(ossNames.ModelView, tempFile.Name);

            // extract SVF from the archive
            ZipFile.ExtractToDirectory(tempFile.Name, localNames.SvfDir, overwriteFiles: true); // TODO: non-default encoding is not supported
        }

        /// <summary>
        /// OSS names for "hashed" files.
        /// Uses default parameters hash if <paramref name="hash"/> is not provided.
        /// </summary>
        public OSSObjectNameProvider GetOssNames(string hash = null) => Project.OssNameProvider(hash ?? Metadata.Hash);

        /// <summary>
        /// Local names for "hashed" files.
        /// Uses default parameters hash if <paramref name="hash"/> is not provided.
        /// </summary>
        public LocalNameProvider GetLocalNames(string hash = null) => Project.LocalNameProvider(hash ?? Metadata.Hash);

        /// <summary>
        /// Delete local cache for the project.
        /// </summary>
        public void DeleteLocal()
        {
            if (Directory.Exists(Project.LocalAttributes.BaseDir))
            {
                Directory.Delete(Project.LocalAttributes.BaseDir, recursive: true);
            }
        }
    }
}
