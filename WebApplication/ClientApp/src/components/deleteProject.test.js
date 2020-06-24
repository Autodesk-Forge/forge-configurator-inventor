import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { DeleteProject } from './deleteProject';

Enzyme.configure({ adapter: new Adapter() });

const props = {
    deleteProjectDlgVisible: true
};

const showDeleteProjectMockFn = jest.fn();
const deleteProjectMockFn = jest.fn();

describe('Delete project confirmation dialog', () => {

  beforeEach(() => {
    showDeleteProjectMockFn.mockClear();
    deleteProjectMockFn.mockClear();
  });


  it('Delete button calls the delete handler', () => {
    const wrapper = shallow(<DeleteProject { ...props } showDeleteProject={showDeleteProjectMockFn} deleteProject={deleteProjectMockFn} />);
    const button = wrapper.find("#delete_ok_button");
    button.simulate('click');
    expect(deleteProjectMockFn).toHaveBeenCalled();
  });


  it('Cancel button closes the dialog', () => {
    const wrapper = shallow(<DeleteProject { ...props } showDeleteProject={showDeleteProjectMockFn} deleteProject={deleteProjectMockFn} />);
    const button = wrapper.find("#cancel_button");
    button.simulate('click');
    expect(showDeleteProjectMockFn).toHaveBeenCalledWith(false);
    expect(deleteProjectMockFn).toHaveBeenCalledTimes(0);
  });
});
