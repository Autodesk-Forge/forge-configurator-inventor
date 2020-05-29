using Microsoft.AspNetCore.Routing;
using WebApplication.Definitions;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Utility class to deal with stuff like URLs in generated DTOs.
    /// </summary>
    public class DtoGenerator
    {
        private readonly ResourceProvider _resourceProvider;
        private readonly LinkGenerator _linkGenerator;

        public DtoGenerator(ResourceProvider resourceProvider, LinkGenerator linkGenerator)
        {
            _resourceProvider = resourceProvider;
            _linkGenerator = linkGenerator;
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
                        Svf = _resourceProvider.ToDataUrl(project.LocalNameProvider(hash).SvfDir),
                        ModelDownloadUrl = modelDownloadUrl
                    };
        }
    }
}
