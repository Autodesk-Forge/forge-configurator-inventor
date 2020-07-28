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

import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Parameter } from './parameter';

Enzyme.configure({ adapter: new Adapter() });

const editboxParam = {
  "name": "editbox",
  "value": "1000 mm",
  "type": "",
  "units": "mm",
  "allowedValues": [],
  "label": "text"
};

const listboxParam = {
  "name": "listbox",
  "value": "green",
  "type": "",
  "units": "",
  "allowedValues": ["reg","green","blue"],
  "label": "combo"
};

const checkboxParam = {
  "name": "checkbox",
  "value": "True",
  "type": "",
  "units": "Boolean",
  "allowedValues": [],
  "label": "check"
};

/** Utility function to extract title from parameter component */
function getTitle(paramWrapper) {

  // checkbox has a special handling
  const checkBoxTextWrapper = paramWrapper.find('.checkboxtext');
  if (checkBoxTextWrapper.length > 0) {
    return checkBoxTextWrapper.text();
  } else {
    return paramWrapper.children().first().text();
  }
}

describe('components', () => {
  describe('parameter', () => {
    it('test editbox', () => {
        const wrapper = shallow(<Parameter parameter={editboxParam}/>);
        const wrapperComponent = wrapper.find('Input');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("value")).toEqual(editboxParam.value);
        expect(wrapperComponent.prop("disabled")).toBeFalsy();
        expect(getTitle(wrapper)).toEqual("text");
      });

    it('test listbox', () => {
        const wrapper = shallow(<Parameter parameter={listboxParam}/>);
        const wrapperComponent = wrapper.find('Dropdown');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("value")).toEqual(listboxParam.value);
        expect(wrapperComponent.prop("disabled")).toBeFalsy();
        expect(getTitle(wrapper)).toEqual("combo");
      });

    it('test checkbox', () => {
        const wrapper = shallow(<Parameter parameter={checkboxParam}/>);
        const wrapperComponent = wrapper.find('Checkbox');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("checked")).toEqual(true);
        expect(wrapperComponent.prop("disabled")).toBeFalsy();
        expect(getTitle(wrapper)).toEqual("check");
      });

      it('test onEditChange called when changed value', () => {
        const editParameterMock = jest.fn();
        const props = {
          parameter: editboxParam,
          activeProject: { id: "1" },
          editParameter: editParameterMock
        };

        const wrapper = shallow(<Parameter {...props}/>);
        const input = wrapper.find('Input');
        expect(input.length).toEqual(1);

        input.simulate('change', {target: {value : '111'}});
        expect(editParameterMock).toHaveBeenCalledWith("1", {"name": "editbox", "value": "111"});
      });

      it('test onComboChange called when changed value', () => {
        const editParameterMock = jest.fn();
        const props = {
          parameter: listboxParam,
          activeProject: { id: "1" },
          editParameter: editParameterMock
        };

        const wrapper = shallow(<Parameter {...props}/>);
        const input = wrapper.find('Dropdown');
        expect(input.length).toEqual(1);

        input.simulate('change', 'blue');
        expect(editParameterMock).toHaveBeenCalledWith("1", {"name": "listbox", "value": "blue"});
      });

      it('test onCheckboxChange called when changed value', () => {
        const editCheckBoxMock = jest.fn();
        const props = {
          parameter: checkboxParam,
          activeProject: { id: "1" },
          editParameter: editCheckBoxMock
        };

        const wrapper = shallow(<Parameter {...props}/>);
        const input = wrapper.find('Checkbox');
        expect(input.length).toEqual(1);

        input.simulate('change', null);
        expect(editCheckBoxMock).toHaveBeenCalledWith("1", {"name": "checkbox", "value": "False"});
        editCheckBoxMock.mockClear();
        input.simulate('change', 'True');
        expect(editCheckBoxMock).toHaveBeenCalledWith("1", {"name": "checkbox", "value": "True"});
      });

      describe('readonly flag', () => {

        /** Create read-only parameter */
        function makeRO(param) {
          return { ...param, readonly: true };
        }

        it.each([
          [ editboxParam, 'Input' ],
          [ listboxParam, 'Dropdown' ],
          [ checkboxParam, 'Checkbox' ]
        ])('ensure `disabled` attribute - case #%#', (param, componentType) => {

          const wrapper = shallow(<Parameter parameter={ makeRO(param) }/>);
          const component = wrapper.find(componentType);
          expect(component.prop("disabled")).toBe(true);
        });
      });
  });
});
