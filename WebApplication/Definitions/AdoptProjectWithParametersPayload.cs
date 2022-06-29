using Shared;

namespace webapplication.Definitions
{
    public class AdoptProjectWithParametersPayload : DefaultProjectConfiguration
    {
        public InventorParameters Config { get; set; } = null!;
    }
}
