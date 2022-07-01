using Shared;

namespace WebApplication.Definitions
{
    public class AdoptProjectWithParametersPayload : DefaultProjectConfiguration
    {
        public InventorParameters Config { get; set; } = null!;
    }
}
