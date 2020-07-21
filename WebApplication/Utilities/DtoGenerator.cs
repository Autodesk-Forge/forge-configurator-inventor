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
        public TProjectDTOBase MakeProjectDTO<TProjectDTOBase>(Project project, string hash) where TProjectDTOBase: ProjectDTOBase, new()
        {
            // TODO: fix workaround for `_linkGenerator` check for null
            var modelDownloadUrl = _linkGenerator?.GetPathByAction(controller: "Download",
                                                                    action: "Model",
                                                                    values: new { projectName = project.Name, hash });

            return new TProjectDTOBase
                    {
                        Svf = _localCache.ToDataUrl(project.LocalNameProvider(hash).SvfDir),
                        ModelDownloadUrl = modelDownloadUrl,
                        Hash = hash
            };
        }

        /// <summary>
        /// Generate project DTO.
        /// </summary>
        public ProjectDTO ToDTO(ProjectStorage projectStorage)
        {
            Project project = projectStorage.Project;

            var dto = MakeProjectDTO<ProjectDTO>(project, projectStorage.Metadata.Hash);
            dto.Id = project.Name;
            dto.Label = project.Name;
            dto.Image = _localCache.ToDataUrl(project.LocalAttributes.Thumbnail);
            return dto;
        }
    }
}
