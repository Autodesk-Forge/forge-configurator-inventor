namespace WebApplication.Processing
{
    /// <summary>
    /// Update project:
    /// - generate SVF
    /// - extract parameters
    /// - save current model
    /// </summary>
    public class UpdateProject : AggregatedDefinition
    {
        private readonly bool _assembly;
        public UpdateProject(Publisher publisher, bool assembly) :
            base(publisher,
                new UpdateParameters(publisher, assembly),
                new CreateSVF(publisher),
                new ExtractParameters(publisher))
        {
            _assembly = assembly;
        }

        public override string Id => $"{nameof(UpdateProject)}{(this._assembly ? "A" : "P")}";
        public override string Description => "Update parameters";
    }
}
