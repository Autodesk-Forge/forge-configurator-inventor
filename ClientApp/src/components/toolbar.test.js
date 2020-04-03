import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import TopNav, {
    Logo,
    LogoText,
    Interactions, 
    ProfileAction,
    NavAction,
    Separator
  } from '@hig/top-nav';

import ProjectAccountSwitcher from '@hig/project-account-switcher';
import {Toolbar} from './toolbar'

Enzyme.configure({ adapter: new Adapter() })

function setup() {
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
    }

    const props = {
        projectList: projectList,
        fetchProjects: () => {}
    }    
    
    const enzymeWrapper = shallow(<Toolbar {...props} />)

    return {
        props,
        enzymeWrapper
    }
}

describe('components', () => {
  describe('Toolbar', () => {
    it('should render self and subcomponents', () => {
      const { props, enzymeWrapper } = setup()

      // there's a TopNav component
      const topnav = enzymeWrapper.find(TopNav);
      expect(topnav.length).toBe(1);

      // there's a Logo component 
      const logo = topnav.props().logo;
      expect(logo.props.label).toBe('Autodesk HIG');
      expect(logo.props.link).toBe('https://forge.autodesk.com');

      // there's a projectAccountSwitcher component with appropriate data on it's props
      const rightactions = topnav.props().rightActions;
      const projectswitcher = rightactions.props.children[0];
      expect(projectswitcher.props.children.props.defaultProject).toBe(props.projectList.activeProjectId);
      expect(projectswitcher.props.children.props.projects).toBe(props.projectList.projects);
      expect(projectswitcher.props.children.props.projectTitle).toBe('Projects');
    })
  })
})