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

namespace webapplication.Definitions
{
    public enum CompletionCheck
    {
        /// <summary>
        /// Use polling (ask about status periodically).
        /// </summary>
        Polling,

        /// <summary>
        /// Use callback URL to get notification from FDA servers.
        /// </summary>
        Callback
    }

    public class PublisherConfiguration
    {
        /// <summary>
        /// How publisher should check for completion.
        /// </summary>
        public CompletionCheck CompletionCheck { get; set; } = CompletionCheck.Polling;

        private string? callbackUrlBase;

        /// <summary>
        /// Base URL for callback.
        /// </summary>
        public string? CallbackUrlBase
        {
            get
            {
                return callbackUrlBase + "/";
            }
            set
            {
                callbackUrlBase = value;
            }
        }
    }
}