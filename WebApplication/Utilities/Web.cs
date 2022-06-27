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

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace webapplication.Utilities
{
    /// <summary>
    /// Web-related utilities.
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Download URL to the local file.
        /// </summary>
        public static async Task DownloadAsync(this HttpClient httpClient, string? url, string? localFile)
        {
            await using var httpStream = await httpClient.GetStreamAsync(url);
            await using var localStream = File.Create(localFile);
            await httpStream.CopyToAsync(localStream);
        }
    }
}
