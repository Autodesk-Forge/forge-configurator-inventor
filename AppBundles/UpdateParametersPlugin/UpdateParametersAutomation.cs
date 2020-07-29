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
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Newtonsoft.Json;
using Shared;

namespace UpdateParametersPlugin
{
    [ComVisible(true)]
    public class UpdateParametersAutomation
    {
        private readonly InventorServer _inventorApplication;

        public UpdateParametersAutomation(InventorServer inventorApp)
        {
            _inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace($"Run called with {doc.DisplayName}");
            LogError("Input arguments are expected. The processing failed.");
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            LogTrace($"RunWithArguments called with {doc.DisplayName} with {map.Count} arguments");

            using (new HeartBeat())
            {
                Parameters parameters;
                switch (doc.DocumentType)
                {
                    case DocumentTypeEnum.kPartDocumentObject:
                        parameters = ((PartDocument) doc).ComponentDefinition.Parameters;
                        break;
                    case DocumentTypeEnum.kAssemblyDocumentObject:
                        parameters = ((AssemblyDocument) doc).ComponentDefinition.Parameters;
                        break;
                    default:
                        LogError($"Unsupported document type: {doc.DocumentType}");
                        return;
                }

                LogTrace("Read parameters");

                string paramFile = (string) map.Value["_1"];
                string json = System.IO.File.ReadAllText(paramFile);
                LogTrace(json);

                var incomingParams = JsonConvert.DeserializeObject<InventorParameters>(json);

                bool changed = false;

                foreach (var pair in incomingParams)
                {
                    string paramName = pair.Key;
                    InventorParameter paramData = pair.Value;

                    // don't try to change readonly properties
                    if (paramData.ReadOnly.GetValueOrDefault(false))
                    {
                        LogTrace($"Skipping readonly '{paramName}'");
                        continue;
                    }

                    try
                    {
                        var userParameter = parameters[paramName];

                        string expression = userParameter.Expression;
                        if (! paramData.Value.Equals(expression))
                        {
                            LogTrace($"Applying '{paramData.Value}' to '{paramName}'");
                            SetExpression(userParameter, paramData);

                            changed = true;
                        }
                        else
                        {
                            LogTrace($"Skipping '{paramName}'");
                        }
                    }
                    catch (Exception e)
                    {
                        // some parameters (maybe when parameter gets driven by iLogic rule) are throwing error on change.
                        // so swallow the thrown error, log about it and continue
                        LogError($"Failed to set '{paramName}' parameter. Error is {e}.");
                    }
                }

                // don't do anything unless parameters really changed
                if (changed)
                {
                    LogTrace("Updating");
                    doc.Update2();

                    LogTrace("Saving");
                    doc.Save2(SaveDependents: true);
                    LogTrace("Closing");
                    doc.Close(true);
                }
                else
                {
                    LogTrace("Update is not required.");
                }
            }
        }

        private static void SetExpression(Parameter parameter, InventorParameter paramData)
        {
            // using strongly typed `parameter.get_Units()` throws:
            //    "Failed to set 'PartMaterial' parameter. Error is System.Runtime.InteropServices.COMException (0x80020003): Member not found. (Exception from HRESULT: 0x80020003 (DISP_E_MEMBERNOTFOUND))"
            // and using dynamic (late binding) helps. So stick with it.
            dynamic dynParameter = parameter;

            string expression  = paramData.Value;

            if (dynParameter.Units != "Text")
            {
                // it's necessary to trim off double quote for non-text parameters
                if (expression.StartsWith("\"") && expression.EndsWith("\""))
                {
                    expression = expression.Substring(1, expression.Length - 2);
                    LogTrace($"Expression normalized to '{expression}'");
                }
            }

            // apply the expression
            dynParameter.Expression = expression;
        }

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}