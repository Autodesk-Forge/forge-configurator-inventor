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
using webapplication.Definitions;

namespace webapplication.Processing
{
    public class UpdateParameters : ForgeAppBase
    {
        private const string OutputModelIAMParameterName = "OutputModelIAMFile";
        private const string OutputModelIPTParameterName = "OutputModelIPTFile";

        /// <summary>
        /// Design Automation parameter name for JSON file with Inventor parameters.
        /// </summary>
        private const string InventorParameters = "InventorParams";

        public override string Id => nameof(UpdateParameters);
        public override string Description => "Update parameters from Inventor document, and extract the results";
        protected override string OutputName => "documentParams.json";
        protected override string OutputUrl(ProcessingArgs projectData) => projectData.ParametersJsonUrl!;

        public override List<string> ActivityCommandLine =>
            new()
            {
                $"$(engine.path)\\InventorCoreConsole.exe /al \"$(appbundles[{ActivityId}].path)\" /ilod \"$(args[{InputDocParameterName}].path)\" /paramFile \"$(args[{InventorParameters}].path)\" /p"
            };

        public override Dictionary<string, Parameter> GetActivityParams() =>
            new()
            {
                {
                    InputDocParameterName,
                    new Parameter { Verb = Verb.Get, Description = "IPT or IAM (in ZIP) file to process" }
                },
                {
                    InventorParameters,
                    new Parameter { Verb = Verb.Get, Description = "JSON file with Inventor parameters" }
                },
                {
                    OutputModelIAMParameterName,
                    new Parameter { Verb = Verb.Put, LocalName = FolderToBeZippedName, Zip = true }
                },
                {
                    OutputModelIPTParameterName,
                    new Parameter { Verb = Verb.Put, LocalName = IptName }
                },
                {
                    OutputParameterName,
                    new Parameter { Verb = Verb.Put, LocalName = OutputName }
                }
            };

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateParameters(Publisher publisher) : base(publisher) {}

        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var workItemArgs = base.ToWorkItemArgs(data);

            UpdateData projectData = (data as UpdateData)!;
            if (projectData.InputParamsUrl != null) // TODO: use generics
            {
                workItemArgs.Add(InventorParameters, new XrefTreeArgument { Url = projectData.InputParamsUrl });
                workItemArgs.Add(OutputModelIAMParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIAMModelUrl, Optional = true });
                workItemArgs.Add(OutputModelIPTParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIPTModelUrl, Optional = true });
            }
            return workItemArgs;
        }
    }
}
