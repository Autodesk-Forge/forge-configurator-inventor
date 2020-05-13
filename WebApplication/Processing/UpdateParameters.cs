using System;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    public class UpdateParameters : ForgeAppBase
    {
        public override string Id => nameof(UpdateParameters);
        public override string Description => "Update parameters from Inventor document";

        protected override string OutputUrl(AdoptionData projectData)
        {
            throw new NotImplementedException();
        }

        protected override string OutputName => "documentParams.json";

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateParameters(Publisher publisher) : base(publisher)
        {
        }
    }
}
