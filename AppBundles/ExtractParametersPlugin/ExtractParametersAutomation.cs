using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ExtractParametersPlugin
{
    [ComVisible(true)]
    public class ExtractParametersAutomation
    {
        private readonly InventorServer inventorApplication;

        public ExtractParametersAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace("Run called with {0}", doc.DisplayName);

            try
            {
                using (new HeartBeat())
                {
                    dynamic dynDoc = doc;
                    string parameters = getParamsAsJson(dynDoc.ComponentDefinition.Parameters.UserParameters);

                    System.IO.File.WriteAllText("documentParams.json", parameters);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        public string getParamsAsJson(dynamic userParameters)
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
                List<object> parameters = new List<object>();
                foreach (dynamic param in userParameters)
                {
                    List<object> paramProperties = new List<object>();
                    if (param.ExpressionList != null)
                    {
                        string[] expressions = param.ExpressionList.GetExpressionList();
                        JArray values = new JArray(expressions);
                        paramProperties.Add(new JProperty("values", values));
                    }
                    paramProperties.Add(new JProperty("value", param.Expression));
                    paramProperties.Add(new JProperty("unit", param.Units));
                    parameters.Add(new JProperty(param.Name, new JObject(paramProperties.ToArray())));
                }
                JObject allParameters = new JObject(parameters.ToArray());
                string paramsJson = allParameters.ToString();
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