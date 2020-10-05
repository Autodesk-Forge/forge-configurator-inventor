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
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// RFA generator from Inventor document.
    /// </summary>
    public class CreateRFA : ForgeAppBase
    {
        public override string Engine { protected set; get; } = "Autodesk.Revit+2020";

        public override string Id => nameof(CreateRFA);
        public override string Description => "Generate RFA from SAT document";

        protected override string OutputUrl(ProcessingArgs projectData) => projectData.RfaUrl;
        protected override string OutputName => "Output.rfa";

        protected internal override ForgeRegistration Registration { get; } = ForgeRegistration.All;

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(engine.path)\\revitcoreconsole.exe /al \"$(appbundles[{ActivityId}].path)\""
            };

        protected override void AddInputArgs(IDictionary<string, IArgument> args, ProcessingArgs data)
        {
            args.Add(InputDocParameterName, new XrefTreeArgument { Url = data.InputDocUrl, LocalName = "Input.sat" });
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateRFA(Publisher publisher) : base(publisher) {}
    }
}
