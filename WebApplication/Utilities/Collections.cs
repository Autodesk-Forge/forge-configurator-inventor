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

namespace webapplication.Utilities
{
    public class Collections
    {
        /// <summary>
        /// Merge dictionaries into a single one.
        /// 
        /// NOTE: in case of several key-value pairs - the last value will "survive".
        /// </summary>
        public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries) where TKey : notnull
        {
            if (dictionaries == null) throw new ArgumentNullException(nameof(dictionaries));
            var output = new Dictionary<TKey, TValue>();

            foreach (var dict in dictionaries)
            {
                foreach (var (key, value) in dict)
                {
                    // to avoid exception on collision
                    output[key] = value;
                }
            }

            return output;
        }
    }
}
