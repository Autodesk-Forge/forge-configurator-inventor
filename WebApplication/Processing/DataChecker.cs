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
using Autodesk.Forge.DesignAutomation.Model;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Universal plugin to extract different kinds of data during adoption:
    /// - drawings list
    /// - unsupported plugins
    /// - (NYI) missing references
    /// </summary>
    public class DataChecker : ForgeAppBase
    {
        public override string Id => nameof(DataChecker);
        public override string Description => "Data checker during adoption";

        // first output argument is JSON with list of drawings in the project
        protected override string OutputUrl(ProcessingArgs projectData) => projectData.DrawingsListUrl;
        protected override string OutputName => "drawings-list.json";
        protected override bool IsOutputZip => false;

        // second output argument is JSON with adoption messages
        private const string MessagesFileName = "adopt-messages.json";
        private const string MessagesParamName = nameof(DataChecker) + "Messages";

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataChecker(Publisher publisher) : base(publisher) {}

        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var args = base.ToWorkItemArgs(data);
            args.Add(MessagesParamName, new XrefTreeArgument { Verb = Verb.Put, Url = data.AdoptMessagesUrl, Optional = false });

            return args;
        }

        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var activityParams = base.GetActivityParams();
            activityParams.Add(MessagesParamName, new Parameter { Verb = Verb.Put, LocalName = MessagesFileName, Zip = false });
            return activityParams;
        }
    }
}
