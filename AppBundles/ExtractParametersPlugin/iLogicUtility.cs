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

using System.Diagnostics;
using System.Linq;
using Autodesk.iLogic.Core.UiBuilderStorage;
using Autodesk.iLogic.UiBuilderCore.Data;
using Autodesk.iLogic.UiBuilderCore.Storage;
using Inventor;
using Newtonsoft.Json;
using Shared;

namespace ExtractParametersPlugin
{
    public class iLogicForm
    {
        public string Name { get; set; }
        public InventorParameters Parameters { get; set; }
    }

    /// <summary>
    /// Read model-specific UI data from iLogic forms that are stored in an Inventor document.
    /// </summary>
    internal class iLogicFormsReader
    {
        #region Inner data types

        private class FormExtractor
        {
            private readonly FormSpecification _formSpec;
            private readonly InventorParameters _knownParameters;
            private readonly Document _document;
            private readonly InventorParameters _collectedParameters = new InventorParameters();

            private FormExtractor(FormSpecification formSpec, InventorParameters knownParameters, Document document)
            {
                _formSpec = formSpec;
                _knownParameters = knownParameters;
                _document = document;
            }

            public static iLogicForm Get(UiStorage storage, string formName, InventorParameters allowedParameters,
                Document document)
            {
                FormSpecification formSpec = storage.LoadFormSpecification(formName);
                var extractor = new FormExtractor(formSpec, allowedParameters, document);
                return extractor.Run();
            }

            private iLogicForm Run()
            {
                ProcessGroup(_formSpec);

                return new iLogicForm
                {
                    Name = _formSpec.Name,
                    Parameters = _collectedParameters
                };
            }

            private void ProcessGroup(UiElementContainerSpec container, UiElementContainerSpec containerToProcess = null)
            {
                if (containerToProcess == null)
                    containerToProcess = container;

                foreach (var elementSpec in containerToProcess.Items)
                {
                    ProcessElement(container, elementSpec);
                }
            }

            private void ProcessElement(UiElementContainerSpec container, UiElementSpec elementSpec)
            {
                switch (elementSpec)
                {
                    case ParameterControlSpec parameterControlSpec:
                        ProcessParameter(parameterControlSpec);
                        break;

                    case iPropertyControlSpec iPropertySpec:
                        Process_iProperty(iPropertySpec);
                        break;

                    case UiElementContainerSpec subContainer:
                        switch (subContainer)
                        {
                            case ControlSpecGroupBase controlGroup:
                                ProcessGroup(controlGroup);
                                break;
                            case ControlRowSpec _:
                                ProcessGroup(container, subContainer);
                                break;
                        }
                        break;
                }
            }

            private void Process_iProperty(iPropertyControlSpec iPropertySpec)
            {
                var json = JsonConvert.SerializeObject(iPropertySpec);
                Trace.WriteLine(json);

                var propertySetName = iPropertySpec.PropertySetName;
                if (_document.PropertySets.PropertySetExists(propertySetName, out dynamic propertySet))
                {
                    var propertyName = iPropertySpec.PropertyName;

                    dynamic property = propertySet.Item(propertyName);
                    if (property == null)
                    {
                        Trace.TraceError($"Cannot find '{propertyName}' property.");
                        return;
                    }

                    var parameter = new InventorParameter
                                    {
                                        Label = iPropertySpec.Name.Trim(),
                                        ReadOnly = iPropertySpec.ReadOnly,
                                        Value = property.Value
                                    };

                    _collectedParameters.Add_iProperty(propertySetName, propertyName, parameter);
                }
                else
                {
                    Trace.TraceError($"Cannot find '{propertySetName}' property set.");
                }
            }

            private void ProcessParameter(ParameterControlSpec spec)
            {
                if (_knownParameters.TryGetValue(spec.ParameterName, out var knownParameter))
                {
                    var parameter = new InventorParameter
                                    {
                                        Label = spec.Name.Trim(),
                                        Unit = knownParameter.Unit,
                                        ReadOnly = spec.ReadOnly,
                                        Value = knownParameter.Value,
                                        Values = knownParameter.Values
                                    };

                    _collectedParameters.Add(spec.ParameterName, parameter);
                }
                else
                {
                    Trace.TraceError($"Processing unknown {spec.ParameterName} parameter");
                }
            }
        }

        #endregion

        private readonly Document _document;
        private readonly InventorParameters _knownParameters;
        private readonly UiStorage _storage;

        /// <summary>Constructor.</summary>
        /// <param name="document">Inventor document.</param>
        /// <param name="knownParameters">Map with Inventor parameters, which are allowed to be extracted.</param>
        public iLogicFormsReader(Document document, InventorParameters knownParameters)
        {
            _document = document;
            _knownParameters = knownParameters;
            _storage = UiStorageFactory.GetDocumentStorage(document);
        }

        public iLogicForm[] Extract()
        {
            return _storage.FormNames
                            .Select(GetGroupsAndParameters)
                            .ToArray();
        }

        private iLogicForm GetGroupsAndParameters(string formName)
        {
            return FormExtractor.Get(_storage, formName, _knownParameters, _document);
        }
    }
}
