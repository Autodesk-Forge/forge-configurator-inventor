/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

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
        public string? Value { get; set; }

        [JsonProperty("unit")]
        public string? Unit { get; set; }

        [JsonProperty("values")]
        public string[]? Values { get; set; }

        [JsonProperty("readonly")]
        public bool? ReadOnly { get; set; }

        [JsonProperty("label")]
        public string? Label { get; set; }

        [JsonProperty("errormessage")]
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Format for data stored in `parameters.json`.
    /// </summary>
    public class InventorParameters : Dictionary<string, InventorParameter>
    {
    }
}
