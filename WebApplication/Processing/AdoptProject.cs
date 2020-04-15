using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Preprocess incoming project:
    /// - generate thumbnail and SVF
    /// - extract parameters
    /// </summary>
    public class AdoptProject : AggregatedDefinition
    {
        public AdoptProject(Publisher publisher) :
            base(publisher,
                    new CreateSVF(publisher),
                    new CreateThumbnail(publisher),
                    new ExtractParameters(publisher))
        {
        }

        public override string Id => nameof(AdoptProject);
        public override string Description => "Adopt Inventor project";

        public Task<WorkItemStatus> DoIAM(AdoptionData projectData)
        {
            // TODO: yuck! very ugly... TO REWRITE!
            var svf = (CreateSVF) Definitions[0];
            var thumb = (CreateThumbnail)Definitions[1];
            var parameters = (ExtractParameters) Definitions[2];

            var allArgs = new[]
            {
                svf.ToIamArguments(projectData.InputUrl, projectData.InputUrl, projectData.SvfUrl),
                thumb.ToIamArguments(projectData.InputUrl, projectData.InputUrl, projectData.ThumbnailUrl),
                parameters.ToIamArguments(projectData.InputUrl, projectData.InputUrl, projectData.ParametersJsonUrl),
            };

            var merged = Collections.MergeDictionaries(allArgs);
            return Run(merged);
        }
    }
}
