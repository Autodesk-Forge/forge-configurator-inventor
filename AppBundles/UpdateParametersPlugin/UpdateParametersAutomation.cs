using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UpdateParametersPlugin
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
                LogTrace("Read parameters");

                string paramFile = (string) map.Value["_1"];
                string json = System.IO.File.ReadAllText(paramFile);

                var parameters = JsonConvert.DeserializeObject<InventorParameters>(json);

                dynamic dynDoc = doc;
                foreach (var pair in parameters)
                {
                    string paramName = pair.Key;
                    InventorParameter paramData = pair.Value;

                    LogTrace($"Applying '{paramData.Value}' to '{paramName}'");

                    try
                    {
                        dynDoc.ComponentDefinition.Parameters.UserParameters[paramName].Expression = paramData.Value;
                    }
                    catch (Exception e)
                    {
                        LogError($"Failed to set '{paramName}' parameter. Error is {e}");
                        throw;
                    }
                }

                LogTrace("Saving");
                doc.Save2(true);
                LogTrace("Closing");
                doc.Close(true);
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