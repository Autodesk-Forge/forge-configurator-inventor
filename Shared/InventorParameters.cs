using System;
using System.Collections.Generic;

#if NETCOREAPP
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
#endif


namespace Shared
{
    public class InventorParameter
    {
#if NETCOREAPP
        [JsonPropertyName("value")]
#else
        [JsonProperty("value")]
#endif
        public string Value { get; set; }

#if NETCOREAPP
        [JsonPropertyName("unit")]
#else
        [JsonProperty("unit")]
#endif
        public string Unit { get; set; }

#if NETCOREAPP
        [JsonPropertyName("values")]
#else
        [JsonProperty("values")]
#endif
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Format for data stored in `parameters.json`.
    /// </summary>
    public class InventorParameters : Dictionary<string, InventorParameter>
    {
    }
}
