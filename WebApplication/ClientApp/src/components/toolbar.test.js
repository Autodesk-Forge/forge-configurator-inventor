import React from 'react';
import Enzyme, { mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Toolbar } from './toolbar';
import { ProjectSwitcher } from './projectSwitcher';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('toolbar', () => {
    it('verify that toolbar contains project switcher', () => {
        const fnMock = jest.fn();
        const props = {
          projectList: {
            activeProjectId: "1",
            projects: [
              { id: "1", label: "label1" }, { id: "2", label: "label2" }

            ]
          },
          fetchProjects: fnMock
        };

        const wrapper = mount(
          <Toolbar>
            <ProjectSwitcher {...props}/>
          </Toolbar>);

        const wrapperComponent = wrapper.find(ProjectSwitcher);
        expect(wrapperComponent.length).toEqual(1);
        expect(fnMock).toHaveBeenCalledTimes(1);
      });
  });
});
