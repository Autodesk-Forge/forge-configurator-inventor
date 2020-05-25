using System.Text.Json.Serialization;

namespace WebApplication.Definitions
{
    public class ProjectStateDTO
    {
        /// <summary>
        /// URL to SVF directory.
        /// </summary>
        [JsonPropertyName("svf")]
        public string Svf { get; set; }

        /// <summary>
        /// Parameters.
        /// </summary>
        [JsonPropertyName("parameters")]
        public InventorParameters Parameters { get; set; }
    }
}
