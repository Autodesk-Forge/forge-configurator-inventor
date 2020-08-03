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
using iLogic;
using Inventor;
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
            private readonly Document _document;
            private readonly InventorParameters _collectedParameters = new InventorParameters();

            private CadPropertiesInRule iPropertyHandler
            {
                get
                {
                    if (_ruleWorker == null)
                    {
                        _ruleWorker = new CadPropertiesInRule(_document, new StylesInEnglishHandler());
                    }
                    return _ruleWorker;
                }
            }
            private CadPropertiesInRule _ruleWorker;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="document"></param>
            /// <param name="formSpec"></param>
            private FormExtractor(Document document, FormSpecification formSpec)
            {
                _formSpec = formSpec;
                _document = document;
            }


            public static iLogicForm Get(Document document, UiStorage storage, string formName)
            {
                FormSpecification formSpec = storage.LoadFormSpecification(formName);
                var extractor = new FormExtractor(document, formSpec);
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
                // Trace.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(iPropertySpec));

                var parameter = new InventorParameter
                                {
                                    Label = iPropertySpec.Name.Trim(),
                                    ReadOnly = iPropertySpec.ReadOnly,
                                    Value = iPropertyHandler.get_Expression(iPropertySpec.PropertySetName, iPropertySpec.PropertyName)
                                };

                _collectedParameters.Add_iProperty(iPropertySpec.PropertySetName, iPropertySpec.PropertyName, parameter);
            }

            private void ProcessParameter(ParameterControlSpec spec)
            {
                Parameter param = CadDocUtil.DocGetParam(_document, spec.ParameterName);
                if (param == null)
                {
                    Trace.TraceError($"Processing unknown {spec.ParameterName} parameter");
                    return;
                }

                InventorParameter parameter = Convert(param);
                parameter.Label = spec.Name.Trim();
                parameter.ReadOnly = spec.ReadOnly;

                _collectedParameters.Add(spec.ParameterName, parameter);
            }
        }

        #endregion

        private readonly Document _document;
        private readonly UiStorage _storage;

        /// <summary>Constructor.</summary>
        /// <param name="document">Inventor document.</param>
        public iLogicFormsReader(Document document)
        {
            _document = document;
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
            return FormExtractor.Get(_document, _storage, formName);
        }

        public static InventorParameter Convert(Parameter parameter)
        {
            dynamic param = parameter; // to avoid failures on methods like get_Units()
            return new InventorParameter
            {
                Unit = param.Units,
                Value = param.Expression,
                Values = param.ExpressionList?.GetExpressionList() ?? new string[0]
            };
        }
    }
}
