﻿/////////////////////////////////////////////////////////////////////
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

using System.IO;
using System.Text;
using System.Text.Json;

namespace WebApplication.Utilities
{
    public static class Json
    {
        /// <summary>
        /// Serialize data as JSON to a stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeIndented">Write indented JSON.</param>
        /// <returns>The stream. Should be disposed.</returns>
        public static MemoryStream ToStream<T>(T data, bool writeIndented = false)
        {
            var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream))
            {
                JsonSerializer.Serialize(jsonWriter, data, typeof(T), new JsonSerializerOptions { WriteIndented = writeIndented, IgnoreNullValues = true });
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Deserialize JSON file content.
        /// </summary>
        public static T DeserializeFile<T>(string filename)
        {
            var content = File.ReadAllText(filename, Encoding.UTF8);
            return JsonSerializer.Deserialize<T>(content);
        }
    }
}
