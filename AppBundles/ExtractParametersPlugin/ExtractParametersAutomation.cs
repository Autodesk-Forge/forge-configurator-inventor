using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ExtractParametersPlugin
{
    public class InventorParameter // TODO: unify its usage
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("values")]
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Format for data stored in `parameters.json`.
    /// </summary>
    public class InventorParameters : Dictionary<string, InventorParameter> {} // TODO: unify its usage


    [ComVisible(true)]
    public class ExtractParametersAutomation
    {
        private readonly InventorServer _inventorApplication;

        public ExtractParametersAutomation(InventorServer inventorApp)
        {
            _inventorApplication = inventorApp;
        }

        public void Run(Document doc)
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
                            parameters = ((PartDocument) doc).ComponentDefinition.Parameters;
                            break;
                        case DocumentTypeEnum.kAssemblyDocumentObject:
                            parameters = ((AssemblyDocument) doc).ComponentDefinition.Parameters;
                            break;
                        default:
                            LogError($"Unsupported document type: {doc.DocumentType}");
                            return;
                    }

                    // extract user parameters
                    InventorParameters userParameters = ExtractParameters(parameters.UserParameters);

                    // save current state
                    LogTrace("Updating");
                    doc.Update2();
                    LogTrace("Saving");
                    doc.Save2(SaveDependents: true);

                    // detect iLogic forms
                    iLogicFormsReader reader = new iLogicFormsReader(doc, userParameters);
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
                        LogTrace("No non-empty iLogic forms found. Using whole list of user parameters.");
                        resultingParameters = userParameters;
                    }

                    // generate resulting JSON. Note it's not formatted (to have consistent hash)
                    string paramsJson = JsonConvert.SerializeObject(resultingParameters, Formatting.None);
                    System.IO.File.WriteAllText("documentParams.json", paramsJson);

                    LogTrace("Closing");
                    doc.Close(true);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        public InventorParameters ExtractParameters(dynamic userParameters)
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
                foreach (dynamic param in userParameters)
                {
                    var parameter = new InventorParameter
                    {
                        Unit = param.Units,
                        Value = param.Expression,
                        Values = param.ExpressionList?.GetExpressionList() ?? new string[0]
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