import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ProjectList } from './projectList';

Enzyme.configure({ adapter: new Adapter() });

const projectList = {
    activeProject: {
        id: '2'
    },
    projects: [
        {
            id: '1',
            label: 'One'
        },
        {
            id: '2',
            label: 'Two'
        },
        {
            id: '3',
            label: 'Three'
        }
    ]
};

const props = {
    projectList: projectList
};

describe('ProjectList components', () => {

  it('Project list has upload button for logged in user', () => {
    const propsWithProfile = { ...props, isLoggedIn: true };
    const wrapper = shallow(<ProjectList { ...propsWithProfile } />);
    const upload = wrapper.find('#projectList_uploadButton');
    expect(upload.prop("className")).not.toContain('hidden');
  });

  it('Project list has NO upload button for anonymous user', () => {
    const propsWithProfile = { ...props, isLoggedIn: false };
    const wrapper = shallow(<ProjectList { ...propsWithProfile } />);
    const upload = wrapper.find('#projectList_uploadButton');
    expect(upload.prop("className")).toContain('hidden');
  });

  it('Project list has NO delete button for anonymous user', () => {
    const propsWithProfile = { ...props, isLoggedIn: false, checkedProjects: [] };
    const wrapper = shallow(<ProjectList { ...propsWithProfile } />);
    const deleteBtn = wrapper.find('#projectList_deleteButton');
    expect(deleteBtn.prop("className")).toContain('hidden');
  });
});
