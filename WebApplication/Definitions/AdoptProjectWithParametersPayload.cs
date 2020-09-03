using Newtonsoft.Json;
using Shared;

namespace WebApplication.Definitions
{
    public class AdoptProjectWithParametersPayload : DefaultProjectConfiguration
    {
        [JsonProperty("config")]
        public InventorParameters Parameters { get; set; }
    }
}
