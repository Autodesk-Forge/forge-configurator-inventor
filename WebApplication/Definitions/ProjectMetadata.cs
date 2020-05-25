using System.Text.Json.Serialization;

namespace WebApplication.Definitions
{
    public class ProjectMetadata
    {
        /// <summary>
        /// Hash string for parameters.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Pathname of the top-level assembly.
        /// </summary>
        [JsonPropertyName("tla")]
        public string TLA { get; set; }
    }
}
