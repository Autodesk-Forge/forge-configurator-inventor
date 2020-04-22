using System.Text.Json.Serialization;

namespace WebApplication.Definitions
{
    public class ProjectAttributes
    {
        /// <summary>
        /// Hash string for parameters.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
