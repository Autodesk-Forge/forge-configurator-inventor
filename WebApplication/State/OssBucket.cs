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
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.Services;

namespace WebApplication.State
{
    /// <summary>
    /// Wrapper to work with OSS bucket.
    /// </summary>
    public class OssBucket
    {
        public string BucketKey { get; }
        private readonly IForgeOSS _forgeOSS;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="forgeOSS">Forge OSS service.</param>
        /// <param name="bucketKey">The bucket key.</param>
        /// <param name="logger">Logger to use.</param>
        public OssBucket(IForgeOSS forgeOSS, string bucketKey, ILogger logger)
        {
            BucketKey = bucketKey;
            _forgeOSS = forgeOSS;
            _logger = logger;
        }

        /// <summary>
        /// Create bucket.
        /// </summary>
        public async Task CreateAsync()
        {
            await _forgeOSS.CreateBucketAsync(BucketKey);
        }

        /// <summary>
        /// Delete the bucket.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await _forgeOSS.DeleteBucketAsync(BucketKey);
        }

        /// <summary>
        /// Deletes the bucket.
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        public async Task DeleteBucketAsync(string bucketName)
        {
            await _forgeOSS.DeleteBucketAsync(bucketName);
        }

        /// <summary>
        /// Get bucket objects.
        /// </summary>
        /// <param name="beginsWith">Search filter ("begin with")</param>
        public async Task<List<ObjectDetails>> GetObjectsAsync(string beginsWith = null)
        {
            return await _forgeOSS.GetBucketObjectsAsync(BucketKey, beginsWith);
        }

        /// <summary>
        /// List all buckets.
        /// </summary>
        /// <returns>List of buckets</returns>
        public async Task<List<string>> GetBucketsAsync()
        {
            return await _forgeOSS.GetBucketsAsync();
        }

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public async Task<string> CreateSignedUrlAsync(string objectName, ObjectAccess access = ObjectAccess.Read, int minutesExpiration = 30)
        {
            return await _forgeOSS.CreateSignedUrlAsync(BucketKey, objectName, access, minutesExpiration);
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public async Task CopyAsync(string fromName, string toName)
        {
            await _forgeOSS.CopyAsync(BucketKey, fromName, toName);
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public async Task DownloadFileAsync(string objectName, string localFullName)
        {
            await _forgeOSS.DownloadFileAsync(BucketKey, objectName, localFullName);
        }

        /// <summary>
        /// Ensure local copy of OSS file.
        /// NOTE: it only checks presence of local file.
        /// </summary>
        public async Task EnsureFileAsync(string objectName, string localFullName)
        {
            if (!File.Exists(localFullName))
            {
                await _forgeOSS.DownloadFileAsync(BucketKey, objectName, localFullName);
            }
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObjectAsync(string oldName, string newName, bool ignoreNotExisting = true)
        {
            try
            {
                await _forgeOSS.RenameObjectAsync(BucketKey, oldName, newName);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                if(ignoreNotExisting)
                {
                    _logger.LogInformation($"Cannot rename object: {oldName} to ${newName} because object doesn't exists which was expected by using ignoreNotExisting = true");
                } else
                {
                    throw;
                }
                
            }
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public async Task DeleteObjectAsync(string objectName)
        {
            await _forgeOSS.DeleteAsync(BucketKey, objectName);
        }

        public async Task UploadObjectAsync(string objectName, Stream stream)
        {
            await _forgeOSS.UploadObjectAsync(BucketKey, objectName, stream);
        }

        public async Task UploadChunkAsync(string objectName, string contentRange, string sessionId, Stream stream)
        {
            // public async Task UploadChunkAsync(string bucketKey, )
            await _forgeOSS.UploadChunkAsync(BucketKey, objectName, contentRange, sessionId, stream);
        }

        public async Task<ApiResponse<dynamic>> GetObjectAsync(string objectName)
        {
            return await _forgeOSS.GetObjectAsync(BucketKey, objectName);
        }

        /// <summary>
        /// Check if bucket contains the object.
        /// </summary>
        public async Task<bool> ObjectExistsAsync(string objectName)
        {
            try
            {
                await CreateSignedUrlAsync(objectName); // don't care about result
                return true;
            }
            catch (ApiException ex) when (ex.ErrorCode == 404)
            {
                // the file is not found. Just swallow the exception
            }

            return false;
        }

        public async Task<string> TryToCreateSignedUrlForReadAsync(string objectName)
        {
            string url = null;
            try
            {
                url = await CreateSignedUrlAsync(objectName);
            }
            catch (ApiException e) when (e.ErrorCode == StatusCodes.Status404NotFound)
            {
                // the file does not exist
            }

            return url;
        }

        /// <summary>
        /// Upload file to OSS.
        /// Uses upload in chunks if necessary.
        /// </summary>
        public async Task SmartUploadAsync(string fileName, string objectName, int chunkMbSize = 5)
        {
            // 2MB is minimal, clamp to it
            chunkMbSize = Math.Max(2, chunkMbSize);
            long chunkSize = chunkMbSize * 1024 * 1024;

            await using var fileReadStream = File.OpenRead(fileName);

            // determine if we need to upload in chunks or in one piece
            long sizeToUpload = fileReadStream.Length;

            // use chunks for all files greater than chunk size
            if (sizeToUpload > chunkSize)
            {
                _logger.LogInformation($"Uploading in {chunkMbSize}MB chunks");

                string sessionId = Guid.NewGuid().ToString();
                long begin = 0;
                byte[] buffer = new byte[chunkSize];

                while (begin < sizeToUpload-1)
                {
                    var bytesRead = await fileReadStream.ReadAsync(buffer, 0, (int)chunkSize);

                    int memoryStreamSize = sizeToUpload - begin < chunkSize ? (int)(sizeToUpload - begin) : (int)chunkSize;
                    await using var chunkStream = new MemoryStream(buffer, 0, memoryStreamSize);
                    
                    var contentRange = $"bytes {begin}-{begin + bytesRead - 1}/{sizeToUpload}";
                    await UploadChunkAsync(objectName, contentRange, sessionId, chunkStream);
                    begin += bytesRead;
                }
            }
            else
            {
                await UploadObjectAsync(objectName, fileReadStream);
            }
        }
    }
}
