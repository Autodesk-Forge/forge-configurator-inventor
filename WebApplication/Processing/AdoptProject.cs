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

using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Generic;
using webapplication.Definitions;

namespace webapplication.Processing
{
    /// <summary>
    /// Adopt incoming project:
    /// - validate incoming data
    /// - extract drawings list
    /// - generate thumbnail, BOM and SVF
    /// - extract parameters
    /// </summary>
    public class AdoptProject : AggregatedDefinition
    {
        private const string OutputModelIAMParameterName = "OutputModelIAMFile";
        private const string OutputModelIPTParameterName = "OutputModelIPTFile";

        public AdoptProject(Publisher publisher) :
            base(publisher, 
                    new DataChecker(publisher),
                    new CreateSVF(publisher),
                    new CreateThumbnail(publisher),
                    new CreateBOM(publisher),
                    new ExtractParameters(publisher))
        {}

        public override string Id => nameof(AdoptProject);
        public override string Description => "Adopt Inventor project";
        public override Dictionary<string, Parameter> GetActivityParams()
        {
            var parameters = base.GetActivityParams();
            parameters.Add(OutputModelIAMParameterName, new Parameter { Verb = Verb.Put, Zip = true, LocalName = FolderToBeZippedName, Required = false });
            parameters.Add(OutputModelIPTParameterName, new Parameter { Verb = Verb.Put, LocalName = IptName, Required = false });
            return parameters;
        }
        public override Dictionary<string, IArgument> ToWorkItemArgs(ProcessingArgs data)
        {
            var args = base.ToWorkItemArgs(data);
            args.Add(OutputModelIAMParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIAMModelUrl, Optional = true });
            args.Add(OutputModelIPTParameterName, new XrefTreeArgument { Verb = Verb.Put, Url = data.OutputIPTModelUrl, Optional = true });
            return args;
        }
    }
}
