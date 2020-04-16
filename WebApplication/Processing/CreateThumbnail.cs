namespace WebApplication.Processing
{
    /// <summary>
    /// Generate PNG thumbnail for Inventor document.
    /// </summary>
    public class CreateThumbnail : ForgeAppBase
    {
        public override string Id => nameof(CreateThumbnail);
        public override string Description => "Generate thumbnail from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            return projectData.SvfUrl;
        }

        protected override string OutputName => "thumbnail.png";

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateThumbnail(Publisher publisher) : base(publisher) { }
    }
}
