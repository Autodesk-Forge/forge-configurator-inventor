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

using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication.Utilities
{
    public class Crypto
    {
        /// <summary>
        /// Generate hex string from the bytes.
        /// </summary>
        public static string BytesToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// Generate hash for the file.
        /// </summary>
        /// <returns>Byte array with hash.</returns>
        public static byte[] GenerateFileHash(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sha1 = CreateHasher();
            return sha1.ComputeHash(stream);
        }

        /// <summary>
        /// Generate hash for the file.
        /// </summary>
        /// <returns>Hash string.</returns>
        public static string GenerateFileHashString(string filePath)
        {
            var bytes = GenerateFileHash(filePath);
            return BytesToString(bytes);
        }

        /// <summary>
        /// Generate hash for stream.
        /// </summary>
        /// <returns>Hash string.</returns>
        public static string GenerateStreamHashString(Stream stream)
        {
            using var sha1 = CreateHasher();
            return BytesToString(sha1.ComputeHash(stream));
        }

        /// <summary>
        /// Generate hash for string.
        /// </summary>
        /// <returns>Hash string.</returns>
        public static string GenerateHashString(string input)
        {
            var buffer = Encoding.UTF8.GetBytes(input);
            using var sha1 = CreateHasher();
            return BytesToString(sha1.ComputeHash(buffer));
        }

        /// <summary>
        /// Generate hash for stringified key/value representation of InventorParameters object.
        /// </summary>
        /// <remarks>It generates hash for the string, parameters are sorted by key, key-value;key-value;....</remarks>
        /// <returns>Hash string.</returns>
        public static string GenerateParametersHashString(InventorParameters parameters)
        {
            // make key-value string ONLY to generate hash
            string keyValuesStr = "";
            foreach (KeyValuePair<string, InventorParameter> param in parameters.OrderBy(kvp => kvp.Key))
            {
                keyValuesStr += $"{param.Key}-{param.Value.Value};";
            }

            return GenerateHashString(keyValuesStr);
        }

        private static HashAlgorithm CreateHasher()
        {
            // IMPORTANT: hasher instance must not be cached!
            // SHA1 was chosen as a hasher because it's shorter
            return SHA1.Create();
        }
    }
}
