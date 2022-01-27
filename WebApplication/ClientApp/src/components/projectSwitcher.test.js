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

import { ProjectSwitcher } from './projectSwitcher';

import Enzyme, { shallow } from 'enzyme';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';

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

const clickEventTarget = {
  target: {
    lastChild: {
      textContent: "Wheel"
    }
  }
};

describe('components', () => {
  describe('Project switcher', () => {

    it('should propagate data to proper properties', () => {
      const wrapper = shallow(<ProjectSwitcher {...baseProps} />);

      const projectSwitcher = wrapper.find('ProjectAccountSwitcher');
      expect(projectSwitcher.props().activeProject).toBe(projectList.activeProjectId);
      expect(projectSwitcher.props().projects).toBe(projectList.projects);
      expect(projectSwitcher.props().projectTitle).toBe('Projects');
    });

    it('should call onChange with given handler',  () => {
      const updateActiveProject = jest.fn();
      const updateActiveTabIndex = jest.fn();
      const fetchParameters = jest.fn();
      const invalidateDrawing = jest.fn();
      const props = {
        updateActiveProject,
        updateActiveTabIndex,
        fetchParameters,
        invalidateDrawing,
        addLog: () => {},
        ... baseProps
      };

      const wrapper = shallow(<ProjectSwitcher {...props} />);
      const projectSwitcher = wrapper.find('ProjectAccountSwitcher');
      projectSwitcher.simulate('click', clickEventTarget);

      expect(updateActiveProject).toHaveBeenLastCalledWith("Wheel");
    });

    it('should activate model tab when project changed', () => {
      const updateActiveProject = jest.fn();
      const updateActiveTabIndex = jest.fn();
      const fetchProjects = jest.fn();
      const fetchParameters = jest.fn();
      const invalidateDrawing = jest.fn();
      const props = {
        updateActiveProject,
        updateActiveTabIndex,
        fetchProjects,
        fetchParameters,
        invalidateDrawing,
        addLog: () => {},
        ... baseProps
      };

      const wrapper = shallow(<ProjectSwitcher {...props} />);
      const projectSwitcher = wrapper.find('ProjectAccountSwitcher');
      projectSwitcher.simulate('click', clickEventTarget);

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