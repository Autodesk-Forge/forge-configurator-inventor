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

namespace WebApplication.Job
{
    public enum ErrorInfoType
    {
        ReportUrl = 0,
        Messages = 1
    }

    public abstract class ProcessingError
    {
        public abstract ErrorInfoType ErrorType { get; }
    }

    public class ReportUrlError : ProcessingError
    {
        public override ErrorInfoType ErrorType { get; } = ErrorInfoType.ReportUrl;
        public string ReportUrl { get; }

        public ReportUrlError(string reportUrl)
        {
            ReportUrl = reportUrl;
        }
    }

    public class MessagesError : ProcessingError
    {
        public override ErrorInfoType ErrorType { get; } = ErrorInfoType.Messages;
        public IEnumerable<string> Messages { get; }

        public MessagesError(params string[] messages)
        {
            Messages = messages ?? Array.Empty<string>();
        }
    }
}
