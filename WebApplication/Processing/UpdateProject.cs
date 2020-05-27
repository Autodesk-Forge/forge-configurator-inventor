namespace WebApplication.Processing
{
    public class UpdateProject : AggregatedDefinition
    {
        public UpdateProject(Publisher publisher) :
            base(publisher,
                new UpdateParameters(publisher),
                new CreateSVF(publisher),
                new ExtractParameters(publisher))
        {
        }

        public override string Id => nameof(UpdateProject);
        public override string Description => "Update parameters";
    }
}
