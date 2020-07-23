using System.Text.Json.Serialization;
using Shared;

namespace WebApplication.Definitions
{
    public class ProjectStateDTO : ProjectDTOBase
    {
        /// <summary>
        /// Parameters.
        /// </summary>
        [JsonPropertyName("parameters")]
        public InventorParameters Parameters { get; set; }
    }
}
