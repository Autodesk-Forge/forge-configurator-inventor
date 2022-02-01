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
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import { CheckboxTableRow } from './checkboxTableRow';

Enzyme.configure({ adapter: new Adapter() });

const checkedProjects = [ '2', '4' ];
const rowData = { id: '2' };

describe('CheckboxTableRow component', () => {
  it('renders checkbox when selectable', () => {
    const wrapper = shallow(<CheckboxTableRow selectable={true} rowData={rowData} checkedProjects={checkedProjects} />);
    expect(wrapper.exists('Checkbox')).toBeTruthy();
  });

  it('does not render checkbox when not selectable', () => {
    const wrapper = shallow(<CheckboxTableRow selectable={false} rowData={rowData} checkedProjects={checkedProjects} />);
    expect(wrapper.exists('Checkbox')).toBeFalsy();
  });

  it('is checked based on passed data', () => {
    const wrapper = shallow(<CheckboxTableRow selectable={true} rowData={rowData} checkedProjects={checkedProjects} />);
    const checkbox = wrapper.find('Checkbox');
    expect(checkbox.prop('checked')).toBeTruthy();
  });

  it('is not checked based on passed data', () => {
    const rowDataOverride = { id: '1' };
    const wrapper = shallow(<CheckboxTableRow rowData={rowDataOverride} selectable={true} checkedProjects={checkedProjects}  />);
    const checkbox = wrapper.find('Checkbox');
    expect(checkbox.prop('checked')).toBeFalsy();
  });

  it('calls onChange with proper data', () => {
    const onChange = jest.fn();
    const wrapper = shallow(<CheckboxTableRow onChange={onChange} selectable={true} rowData={rowData} checkedProjects={checkedProjects} />);
    const checkbox = wrapper.find('Checkbox');
    checkbox.simulate('change', false);
    expect(onChange).toBeCalledWith(false, rowData);
  });
});
