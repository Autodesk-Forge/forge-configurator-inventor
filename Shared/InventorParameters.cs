using System;
using System.Collections.Generic;

// Data types from this file are shared between .NET 4.7+ and netcore projects,
// so we need to have different attributes for Newtonsoft and netcore Json libraries.

#if NETCOREAPP
using JsonProperty = System.Text.Json.Serialization.JsonPropertyNameAttribute;
#else
using JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
#endif


namespace Shared
{
    public class InventorParameter
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("values")]
        public string[] Values { get; set; }

        [JsonProperty("readonly")]
        public bool? ReadOnly { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }

    /// <summary>
    /// Format for data stored in `parameters.json`.
    /// </summary>
    public class InventorParameters : Dictionary<string, InventorParameter>
    {
    }
}
