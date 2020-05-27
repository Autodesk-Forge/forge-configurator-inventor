namespace WebApplication.Processing
{
    public class UpdateProject : AggregatedDefinition
    {
        public UpdateProject(Publisher publisher) :
            base(publisher,
                new UpdateParameters(publisher),
                new CreateSVF(publisher),
                new ExtractParameters(publisher)//,
                //new SaveModel(publisher)
                )
        {
        }

        public override string Id => nameof(UpdateProject);
        public override string Description => "Update parameters";
    }

    public class SaveModel : ForgeAppBase
    {
        public SaveModel(Publisher publisher) : base(publisher) {}

        public override string Id => nameof(SaveModel);
        public override string Description => "Save Inventor model in the current state";
    }
}
