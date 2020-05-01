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
    }
}
