using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Path = System.IO.Path;

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
            LogTrace("Run called with {0}", doc.DisplayName);

            try
            {
                using (new HeartBeat())
                {
                    dynamic dynDoc = doc;
                    string paramsJson = GetParamsAsJson(dynDoc.ComponentDefinition.Parameters.UserParameters);

                    System.IO.File.WriteAllText("documentParams.json", paramsJson);

                    //// save current state
                    //LogTrace("Updating");
                    //doc.Update2();
                    //LogTrace("Saving");
                    //doc.Save2(SaveDependents: true);
                    //LogTrace("Closing");
                    //doc.Close(true);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        public string GetParamsAsJson(dynamic userParameters)
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

                // generate resulting JSON. Note it's not formatted (to have consistent hash)
                string paramsJson = JsonConvert.SerializeObject(parameters, Formatting.None);
                LogTrace(paramsJson);

                return paramsJson;
            }
            catch (Exception e)
            {
                LogError("Error reading params: " + e.Message);
                return "";
            }
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