using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApplication.Controllers
{
    public class InventorParameter
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("values")]
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Format for data stored in `parameters.json`.
    /// </summary>
    public class InventorParameters : Dictionary<string, InventorParameter> {}
}
