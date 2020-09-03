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

using Microsoft.AspNetCore.Routing;
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
        public TProjectDTOBase MakeProjectDTO<TProjectDTOBase>(ProjectStorage projectStorage, string hash, FdaStatsDTO stats) where TProjectDTOBase: ProjectDTOBase, new()
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
                        IsAssembly = projectStorage.IsAssembly,
                        HasDrawing = projectStorage.Metadata.HasDrawings,
                        Stats = stats
                    };
        }

        /// <summary>
        /// Generate project DTO.
        /// </summary>
        public ProjectDTO ToDTO(ProjectStorage projectStorage, FdaStatsDTO stats)
        {
            Project project = projectStorage.Project;

            var dto = MakeProjectDTO<ProjectDTO>(projectStorage, projectStorage.Metadata.Hash, stats);
            dto.Id = project.Name;
            dto.Label = project.Name;
            dto.Image = _localCache.ToDataUrl(project.LocalAttributes.Thumbnail);
            return dto;
        }
    }
}
