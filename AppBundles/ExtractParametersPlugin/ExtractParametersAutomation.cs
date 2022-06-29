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
using System.Runtime.InteropServices;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Inventor;
using PluginUtilities;
using Shared;

namespace ExtractParametersPlugin
{
    [ComVisible(true)]
    public class ExtractParametersAutomation : AutomationBase
    {
        public ExtractParametersAutomation(InventorServer inventorApp) : base(inventorApp)
        {
        }

        public override void ExecWithArguments(Document doc, NameValueMap map)
        {
            LogTrace($"Run called with {doc.DisplayName}");

            try
            {
                using (new HeartBeat())
                {
                    Parameters parameters;
                    switch (doc.DocumentType)
                    {
                        case DocumentTypeEnum.kPartDocumentObject:
                            parameters = (doc as PartDocument).ComponentDefinition.Parameters;
                            break;
                        case DocumentTypeEnum.kAssemblyDocumentObject:
                            parameters = (doc as AssemblyDocument).ComponentDefinition.Parameters;
                            break;
                        default:
                            LogError($"Unsupported document type: {doc.DocumentType}");
                            return;
                    }

                    var paramsExtractor = new ParametersExtractor();
                    paramsExtractor.Extract(doc, parameters);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }
     }
}