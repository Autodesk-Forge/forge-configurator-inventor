import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
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
