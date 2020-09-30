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
using System.Diagnostics;
using System.Linq;
using Inventor;
using Newtonsoft.Json;
using Shared;

namespace PluginUtilities
{
    public class ParametersExtractor
    {
        public void Extract(Document doc, Parameters parameters, InventorParameters updateResultParameters = null)
        {
            // extract user parameters
            InventorParameters allParams = ExtractParameters(doc, parameters);

            // save current state
            LogTrace("Updating");
            doc.Update2();
            LogTrace("Saving");
            doc.Save2(SaveDependents: true);

            // detect iLogic forms
            iLogicFormsReader reader = new iLogicFormsReader(doc, allParams);
            iLogicForm[] forms = reader.Extract();
            LogTrace($"Found {forms.Length} iLogic forms");
            foreach (var form in forms)
            {
                LogTrace($" - {form.Name}");
            }

            // Choose set of parameters to use with the following algorithm:
            // - extract all iLogic forms from the document
            //   - keep only 'user parameters' from a form
            // - use _first_ iLogic form with non-empty list of parameters
            // - if no forms - use UserParameters from the document
            InventorParameters resultingParameters;
            var candidate = forms.FirstOrDefault(form => form.Parameters.Count > 0);
            if (candidate != null)
            {
                LogTrace($"Using '{candidate.Name}' form as a parameter filter");
                resultingParameters = candidate.Parameters;
            }
            else
            {
                LogTrace("No non-empty iLogic forms found. Using all user parameters.");
                resultingParameters = ExtractParameters(doc, parameters.UserParameters);
            }

            // if parameters from an update result were passed in, if validation errors were set on any replace values with them
            if (updateResultParameters != null)
            {
                foreach (KeyValuePair<string, InventorParameter> updateResultParam in updateResultParameters)
                {
                    if (updateResultParam.Value.ErrorMessage != null)
                    {
                        resultingParameters[updateResultParam.Key] = updateResultParam.Value;
                    }
                }
            }

            // generate resulting JSON. Note it's not formatted (to have consistent hash)
            string paramsJson = JsonConvert.SerializeObject(resultingParameters, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None });
            System.IO.File.WriteAllText("documentParams.json", paramsJson);

            LogTrace("Closing");
            doc.Close(true);
        }

        private InventorParameters ExtractParameters(Document doc, dynamic userParameters)
        {
            /* The resulting json will be like this:
              { 
                "length" : {
                  "unit": "in",
                  "value": "10 in",
                  "values": ["5 in", "10 in", "15 in"]
                },
                "width": {
                  "unit": "in",
                  "value": "20 in",
                }
              }
            */
            try
            {
                var parameters = new InventorParameters();
                var docUnitsOfMeasure = doc.UnitsOfMeasure;
                foreach (dynamic param in userParameters)
                {
                    var nominalValue = param.Expression;
                    string errorMessage = null;
                    try
                    {
                        var unitType = docUnitsOfMeasure.GetTypeFromString(param.Units);
                        LogTrace($"Checking expression validity on extraction for {param.Expression} and unit type {param.Units} / {unitType}");
                        if (docUnitsOfMeasure.IsExpressionValid(param.Expression, unitType))
                        {
                            var value = docUnitsOfMeasure.GetValueFromExpression(param.Expression, unitType);
                            nominalValue = docUnitsOfMeasure.GetPreciseStringFromValue(value, unitType);
                        }
                        else
                        {
                            LogTrace($"Expression '{param.Expression}' invalid for unit type '{unitType}' on extract attempt");
                            errorMessage = "Parameter's expression is not valid for its unit type";
                        }
                    }
                    // not all unitTypes seem to be convertible (e.g. kTextUnits). In that case, we'll go on with param.Expression assigned before.
                    catch (Exception e)
                    {
                        LogError("Can't get nominalValue for " + param.Name + ": " + e.Message);
                    }

                    var parameter = new InventorParameter
                    {
                        Unit = param.Units,
                        Value = nominalValue,
                        Values = param.ExpressionList?.GetExpressionList() ?? new string[0],
                        ErrorMessage = errorMessage
                    };
                    parameters.Add(param.Name, parameter);
                }

                return parameters;
            }
            catch (Exception e)
            {
                LogError("Error reading params: " + e.Message);
                return null;
            }
        }

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        public void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        public void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}
