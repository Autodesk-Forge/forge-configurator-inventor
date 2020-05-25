import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Parameter } from './parameter';

Enzyme.configure({ adapter: new Adapter() });

const param1 = {
  "name": "editbox",
  "value": "1000 mm",
  "type": "",
  "units": "mm",
  "allowedValues": []
};

const param2 = {
  "name": "listbox",
  "value": "green",
  "type": "",
  "units": "",
  "allowedValues": ["reg","green","blue"]
};

const param3 = {
  "name": "checkbox",
  "value": "True",
  "type": "",
  "units": "Boolean",
  "allowedValues": []
};

describe('components', () => {
  describe('parameter', () => {
    it('test editbox', () => {
        const wrapper = shallow(<Parameter parameter={param1}/>);
        const wrapperComponent = wrapper.find('Input');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("value")).toEqual(param1.value);
      });
    it('test listbox', () => {
        const wrapper = shallow(<Parameter parameter={param2}/>);
        const wrapperComponent = wrapper.find('Dropdown');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("value")).toEqual(param2.value);
      });
    it('test checkbox', () => {
        const wrapper = shallow(<Parameter parameter={param3}/>);
        const wrapperComponent = wrapper.find('Checkbox');
        expect(wrapperComponent.length).toEqual(1);
        expect(wrapperComponent.prop("checked")).toEqual(true);
      });
  });
});
