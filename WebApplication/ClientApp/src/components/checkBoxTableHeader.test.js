import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { CheckboxTableHeader } from './checkboxTableHeader';

Enzyme.configure({ adapter: new Adapter() });

const projects = [ '1', '2', '4' ];
const checkedProjects = [ '2', '4' ];

describe('CheckboxTableRow component', () => {
  it('renders checkbox when has some checked projects', () => {
    const wrapper = shallow(<CheckboxTableHeader selectable={true} projects={projects} checkedProjects={checkedProjects} />);
    expect(wrapper.exists('Checkbox')).toBeTruthy();
  });

  it('renders checkbox when selectable', () => {
    const wrapper = shallow(<CheckboxTableHeader selectable={true} projects={projects} checkedProjects={[]} />);
    expect(wrapper.exists('Checkbox')).toBeTruthy();
  });

  it('does not render checkbox when not selectable', () => {
    const wrapper = shallow(<CheckboxTableHeader selectable={false} projects={projects} checkedProjects={checkedProjects} />);
    expect(wrapper.exists('Checkbox')).toBeFalsy();
  });

  it('is checked based on passed data', () => {
    const wrapper = shallow(<CheckboxTableHeader selectable={true} projects={projects} checkedProjects={projects} />);
    const checkbox = wrapper.find('Checkbox');
    expect(checkbox.prop('checked')).toBeTruthy();
    expect(checkbox.prop('indeterminate')).toBeFalsy();
  });

  it('is indeterminate based on passed data', () => {
    const wrapper = shallow(<CheckboxTableHeader selectable={true} projects={projects} checkedProjects={checkedProjects} />);
    const checkbox = wrapper.find('Checkbox');
    expect(checkbox.prop('checked')).toBeTruthy();
    expect(checkbox.prop('indeterminate')).toBeTruthy();
  });

  it('is not checked based on passed data', () => {
    const wrapper = shallow(<CheckboxTableHeader projects={projects}  selectable={true} checkedProjects={[]}  />);
    const checkbox = wrapper.find('Checkbox');
    expect(checkbox.prop('checked')).toBeFalsy();
    expect(checkbox.prop('indeterminate')).toBeFalsy();
  });

  describe('calls onChange with proper data', () => {
    it('goes from some selected to all selected', () => {
        const onChange = jest.fn();
        const wrapper = shallow(<CheckboxTableHeader onChange={onChange} selectable={true} projects={projects} checkedProjects={checkedProjects} />);
        const checkbox = wrapper.find('Checkbox');
        checkbox.simulate('change', true);
        expect(onChange).toBeCalledWith(false);
      });

      it('goes from none selected to all selected', () => {
        const onChange = jest.fn();
        const wrapper = shallow(<CheckboxTableHeader onChange={onChange} selectable={true} projects={projects} checkedProjects={[]} />);
        const checkbox = wrapper.find('Checkbox');
        checkbox.simulate('change', true);
        expect(onChange).toBeCalledWith(false);
      });


      it('goes from all selected to none selected', () => {
        const onChange = jest.fn();
        const wrapper = shallow(<CheckboxTableHeader onChange={onChange} selectable={true} projects={projects} checkedProjects={projects} />);
        const checkbox = wrapper.find('Checkbox');
        checkbox.simulate('change', false);
        expect(onChange).toBeCalledWith(true);
      });
  });

});
