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

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Autodesk.Forge.DesignAutomation.Inventor.Utils.Helpers;
using Newtonsoft.Json;
using Shared;
using PluginUtilities;

namespace UpdateParametersPlugin
{
    [ComVisible(true)]
    public class UpdateParametersAutomation : AutomationBase
    {
        public UpdateParametersAutomation(InventorServer inventorApp) : base(inventorApp)
        {
        }

        public override void ExecWithArguments(Document doc, NameValueMap map)
        {
            LogTrace($"ExecWithArguments called with {doc.DisplayName} with {map.Count} arguments");

            using (new HeartBeat())
            {
                Parameters parameters;
                switch (doc.DocumentType)
                {
                    case DocumentTypeEnum.kPartDocumentObject:
                        parameters = ((PartDocument)doc).ComponentDefinition.Parameters;
                        break;
                    case DocumentTypeEnum.kAssemblyDocumentObject:
                        parameters = ((AssemblyDocument)doc).ComponentDefinition.Parameters;
                        break;
                    default:
                        LogError($"Unsupported document type: {doc.DocumentType}");
                        return;
                }

                LogTrace("Read parameters");

                string paramFile = map.AsString("paramFile");
                string json = System.IO.File.ReadAllText(paramFile);
                LogTrace(json);

                var incomingParams = JsonConvert.DeserializeObject<InventorParameters>(json);

                foreach (var pair in incomingParams)
                {
                    string paramName = pair.Key;
                    InventorParameter paramData = pair.Value;

                    try
                    {
                        var userParameter = parameters[paramName];

                        string expression = userParameter.Expression;
                        if (!paramData.Value.Equals(expression))
                        {
                            LogTrace($"Applying '{paramData.Value}' to '{paramName}'");
                            SetExpression(userParameter, paramData, doc);
                            if (paramData.ErrorMessage != null)
                            {
                                // there was an invalid expression the UI will prompt the user to fix, so don't bother changing the rest of the parameters
                                LogTrace("Skip update for the remaining parameters");
                                break;
                            }
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

                var paramsExtractor = new ParametersExtractor();
                paramsExtractor.Extract(doc, parameters, incomingParams);
            }
        }

        private static void SetExpression(Parameter parameter, InventorParameter paramData, Document doc)
        {
            // using strongly typed `parameter.get_Units()` throws:
            //    "Failed to set 'PartMaterial' parameter. Error is System.Runtime.InteropServices.COMException (0x80020003): Member not found. (Exception from HRESULT: 0x80020003 (DISP_E_MEMBERNOTFOUND))"
            // and using dynamic (late binding) helps. So stick with it.
            dynamic dynParameter = parameter;

            string expression = paramData.Value;

            if (dynParameter.Units != "Text")
            {
                // it's necessary to trim off double quote for non-text parameters
                if (expression.StartsWith("\"") && expression.EndsWith("\""))
                {
                    expression = expression.Substring(1, expression.Length - 2);
                    LogTrace($"Expression normalized to '{expression}'");
                }
            }
            else
            {
                // on the other hand, expression validation will fail without the double quotes for text parameters
                if (!expression.StartsWith("\""))
                {
                    expression = "\"" + expression;
                }
                if (!expression.EndsWith("\""))
                {
                    expression = expression + "\"";
                }
            }

            var docUnitsOfMeasure = doc.UnitsOfMeasure;
            LogTrace($"Updating the value for the unit type: {dynParameter.Units}");
            if (docUnitsOfMeasure.IsExpressionValid(expression, dynParameter.Units))
            {
                // apply the expression
                paramData.ErrorMessage = null;
                dynParameter.Expression = expression;
            }
            else
            {
                LogTrace($"Expression '{expression}' is invalid and can't be updated");
                paramData.ErrorMessage = "Parameter's expression is not valid for its unit type";
            }
        }
    }
}