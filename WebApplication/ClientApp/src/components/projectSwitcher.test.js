import React from 'react';

import { ProjectSwitcher } from './projectSwitcher';

import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

Enzyme.configure({ adapter: new Adapter() });

const projectList = {
  activeProjectId: '2',
  projects: [
      {
          id: '1',
          label: 'New Project',
          image: 'new_image.png'
      },
      {
          id: '2',
          label: 'New Project B',
          image: 'new_image_B.png'
      }]
};

const baseProps = {
  projectList,
  fetchProjects: () => {}
};

describe('components', () => {
  describe('Project switcher', () => {

    it('should propagate data to proper properties', () => {
      const wrapper = shallow(<ProjectSwitcher {...baseProps} />);

      expect(wrapper.props().defaultProject).toBe(projectList.activeProjectId);
      expect(wrapper.props().projects).toBe(projectList.projects);
      expect(wrapper.props().projectTitle).toBe('Projects');
    });

    it('should call onChange with given handler',  () => {
      const updateActiveProject = jest.fn();
      const updateActiveTabIndex = jest.fn();
      const fetchParameters = jest.fn();
      const props = {
        updateActiveProject,
        updateActiveTabIndex,
        fetchParameters,
        addLog: () => {},
        ... baseProps
      };

      const wrapper = shallow(<ProjectSwitcher {...props} />);
      wrapper.simulate('change', {
        project : {
          id: 5
        }
      });

      expect(updateActiveProject).toHaveBeenLastCalledWith(5);
      //expect(fetchParameters).toHaveBeenLastCalledWith(5);
    });

    it('should activate model tab when project changed', () => {
      const updateActiveProject = jest.fn();
      const updateActiveTabIndex = jest.fn();
      const fetchProjects = jest.fn();
      const fetchParameters = jest.fn();
      const props = {
        updateActiveProject,
        updateActiveTabIndex,
        fetchProjects,
        fetchParameters,
        addLog: () => {},
        ... baseProps
      };

      const wrapper = shallow(<ProjectSwitcher {...props} />);
      wrapper.simulate('change', {
        project : {
          id: 2
        }
      });

      expect(updateActiveTabIndex).toHaveBeenCalledWith(1); // model tab index
    });
  });

  it('should start loading projects on mount', () => {
    const fetchProjects = jest.fn();

    const props = {
      ... baseProps,
      fetchProjects
    };

    /*const wrapper = */shallow(<ProjectSwitcher {...props} />);

    expect(fetchProjects).toBeCalled();
  });
});