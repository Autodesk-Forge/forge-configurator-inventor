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
  "allowedValues": []
};

const listboxParam = {
  "name": "listbox",
  "value": "green",
  "type": "",
  "units": "",
  "allowedValues": ["reg","green","blue"]
};

const checkboxParam = {
  "name": "checkbox",
  "value": "True",
  "type": "",
  "units": "Boolean",
  "allowedValues": []
};

describe('components', () => {
  describe('parameter', () => {
    it('test editbox', () => {
        const wrapper = shallow(<Parameter parameter={editboxParam}/>);
        const wrapperComponent = wrapper.find('Input');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("value")).toEqual(editboxParam.value);
        expect(wrapperComponent.prop("disabled")).toBeFalsy();
      });
    it('test listbox', () => {
        const wrapper = shallow(<Parameter parameter={listboxParam}/>);
        const wrapperComponent = wrapper.find('Dropdown');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("value")).toEqual(listboxParam.value);
        expect(wrapperComponent.prop("disabled")).toBeFalsy();
      });
    it('test checkbox', () => {
        const wrapper = shallow(<Parameter parameter={checkboxParam}/>);
        const wrapperComponent = wrapper.find('Checkbox');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("checked")).toEqual(true);
        expect(wrapperComponent.prop("disabled")).toBeFalsy();
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

        /** Create readonly parameter */
        function makeRO(param) {
          return { ...param, readonly: true };
        }

        it('checks disabled editbox', () => {

          const component = shallow(<Parameter parameter={ makeRO(editboxParam) }/>).find('Input');
          expect(component.prop("disabled")).toBe(true);
        });

        it('checks disabled listbox', () => {

          const wrapperComponent = shallow(<Parameter parameter={ makeRO(listboxParam) }/>).find('Dropdown');
          expect(wrapperComponent.prop("disabled")).toBe(true);
        });

        it('checks disabled checkbox', () => {

          const wrapperComponent = shallow(<Parameter parameter={ makeRO(checkboxParam) }/>).find('Checkbox');
          expect(wrapperComponent.prop("disabled")).toEqual(true);
        });
      });
  });
});
