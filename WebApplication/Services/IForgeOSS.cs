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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.Model;

namespace WebApplication.Services
{
    public enum ObjectAccess
    {
        Read,
        Write,
        ReadWrite
    }

    public interface IForgeOSS
    {
        /// <summary>
        /// Forge configuration.
        /// </summary>
        ForgeConfiguration Configuration { get; }

        Task<List<ObjectDetails>> GetBucketObjectsAsync(string bucketKey, string beginsWith = null);
        Task<List<string>> GetBucketsAsync();
        Task CreateBucketAsync(string bucketKey);
        Task DeleteBucketAsync(string bucketKey);
        Task UploadObjectAsync(string bucketKey, string objectName, Stream stream);
        Task UploadChunkAsync(string bucketKey, string objectName, string contentRange, string sessionId, Stream stream);

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        Task<string> CreateSignedUrlAsync(string bucketKey, string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30);

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        Task RenameObjectAsync(string bucketKey, string oldName, string newName);

        Task<Autodesk.Forge.Client.ApiResponse<dynamic>> GetObjectAsync(string bucketKey, string objectName);

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        Task CopyAsync(string bucketKey, string fromName, string toName);

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        Task DeleteAsync(string bucketKey, string objectName);

        /// <summary>
        /// Download OSS file.
        /// </summary>
        Task DownloadFileAsync(string bucketKey, string objectName, string localFullName);

        /// <summary>
        /// Get profile for the user with the access token.
        /// </summary>
        /// <param name="token">Oxygen access token.</param>
        /// <returns>Dynamic object with User Profile</returns>
        /// <remarks>
        /// User Profile fields: https://forge.autodesk.com/en/docs/oauth/v2/reference/http/users-@me-GET/#body-structure-200
        /// </remarks>
        Task<dynamic> GetProfileAsync(string token);
    }
}
