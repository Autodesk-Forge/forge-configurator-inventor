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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;
using Shared;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.State;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Utility class to deal with stuff like URLs in generated DTOs.
    /// </summary>
    public class DtoGenerator
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly LocalCache _localCache;

        public DtoGenerator(LinkGenerator linkGenerator, LocalCache localCache)
        {
            _linkGenerator = linkGenerator;
            _localCache = localCache;
        }

        /// <summary>
        /// Create ProjectDTOBase based DTO and fill its properties.
        /// </summary>
        public TProjectDTOBase MakeProjectDTO<TProjectDTOBase>(ProjectStorage projectStorage, string hash) where TProjectDTOBase: ProjectDTOBase, new()
        {
            Project project = projectStorage.Project;
            // TODO: fix workaround for `_linkGenerator` check for null
            var modelDownloadUrl = _linkGenerator?.GetPathByAction(controller: "Download",
                                                                    action: "Model",
                                                                    values: new { projectName = project.Name, hash });

            var bomDownloadUrl = _linkGenerator?.GetPathByAction(controller: "Download",
                                                                    action: "BOM",
                                                                    values: new { projectName = project.Name, hash });

            var bomJsonUrl = _linkGenerator?.GetPathByAction(controller: "ProjectData",
                                                                    action: "GetBOM",
                                                                    values: new { projectName = project.Name, hash });

            var localNames = project.LocalNameProvider(hash);
            return new TProjectDTOBase
            {
                Svf = _localCache.ToDataUrl(localNames.SvfDir),
                        BomDownloadUrl = bomDownloadUrl,
                        BomJsonUrl = bomJsonUrl,
                        ModelDownloadUrl = modelDownloadUrl,
                        Hash = hash,
                    };
        }

        /// <summary>
        /// Generate project DTO.
        /// </summary>
        public ProjectDTO ToDTO(ProjectStorage projectStorage)
        {
            Project project = projectStorage.Project;
            var localAttributes = project.LocalAttributes;

            var dto = MakeProjectDTO<ProjectDTO>(projectStorage, projectStorage.Metadata.Hash);
            dto.Id = project.Name;
            dto.Label = Regex.Replace(project.Name, @"[^\u0000-\u007F]", m => String.Format("u{0}", ((int)m.Value[0]).ToString("X4")));
            dto.Image = _localCache.ToDataUrl(localAttributes.Thumbnail);
            dto.IsAssembly = projectStorage.IsAssembly;
            dto.HasDrawing = projectStorage.Metadata.HasDrawings;
            dto.DrawingsListUrl = _localCache.ToDataUrl(localAttributes.DrawingsList);

            // fill array with adoption messages
            if (File.Exists(localAttributes.AdoptMessages))
            {
                // we are interested only in warning messages
                var allMessages = Json.DeserializeFile<List<Message>>(localAttributes.AdoptMessages);
                dto.AdoptWarnings = allMessages
                    .Where(m => m.Severity == Severity.Warning)
                    .Select(m => m.Text)
                    .ToArray();
            }
            else
            {
                dto.AdoptWarnings = Array.Empty<string>();
            }

            return dto;
        }
    }
}
