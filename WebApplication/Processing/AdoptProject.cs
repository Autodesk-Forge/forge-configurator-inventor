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
    }
}
