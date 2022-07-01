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
using System.Threading.Tasks;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Transfer input to output
    /// </summary>
    public class TransferData : ForgeAppBase
    {
        public TransferData(Publisher publisher) : base(publisher) {}

        public override string Id => nameof(TransferData);
        public override string Description => "Transfer input to output";

        protected internal override ForgeRegistration Registration { get; } = ForgeRegistration.All;

        public override List<string> ActivityCommandLine =>
            new()
            {
                $"\"$(appbundles[{ActivityId}].path)\\EmptyExePlugin.bundle\\Contents\\EmptyExePlugin.exe\""
            };

        public override Dictionary<string, Parameter> GetActivityParams() =>
            new()
            {
                {
                    "source",
                    new Parameter {Verb = Verb.Get, Description = "source location", LocalName = "fileForTransfer"}
                },
                {
                    "target",
                    new Parameter {Verb = Verb.Put, Description = "target location", LocalName = "fileForTransfer"}
                }
            };

        public Task<ProcessingResult> ProcessAsync(string? source, string? target)
        {
            var workItemArgs = new Dictionary<string, IArgument>
            {
                { "source", new XrefTreeArgument { Url = source } },
                { "target", new XrefTreeArgument { Verb = Verb.Put, Url = target } }
            };

            return RunAsync(workItemArgs!);
        }
    }
}
