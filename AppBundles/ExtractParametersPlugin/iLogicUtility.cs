/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
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

using System.Linq;
using Autodesk.iLogic.Core.UiBuilderStorage;
using Autodesk.iLogic.UiBuilderCore.Data;
using Autodesk.iLogic.UiBuilderCore.Storage;
using Inventor;

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
            private readonly InventorParameters _allowedParameters;
            private readonly InventorParameters _collectedParameters = new InventorParameters();

            private FormExtractor(FormSpecification formSpec, InventorParameters allowedParameters)
            {
                _formSpec = formSpec;
                _allowedParameters = allowedParameters;
            }

            public static iLogicForm Get(UiStorage storage, string formName, InventorParameters allowedParameters)
            {
                FormSpecification formSpec = storage.LoadFormSpecification(formName);
                var extractor = new FormExtractor(formSpec, allowedParameters);
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

            private void ProcessParameter(ParameterControlSpec pcs)
            {
                if (_allowedParameters.TryGetValue(pcs.ParameterName, out var knownParameter))
                {
                    _collectedParameters.Add(pcs.ParameterName, knownParameter);
                }
            }
        }

        #endregion

        private readonly InventorParameters _allowedParameters;
        private readonly UiStorage _storage;

        /// <summary>Constructor.</summary>
        /// <param name="document">Inventor document.</param>
        /// <param name="allowedParameters">Map with Inventor parameters, which are allowed to be extracted.</param>
        public iLogicFormsReader(Document document, InventorParameters allowedParameters)
        {
            _allowedParameters = allowedParameters;
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
            return FormExtractor.Get(_storage, formName, _allowedParameters);
        }
    }
}
