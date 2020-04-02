import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import Tabs, { Tab } from "@hig/tabs";
import App from '../App'

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
    
    const enzymeWrapper = shallow(<App {...props} />)

    return {
        props,
        enzymeWrapper
    }
}

describe('components', () => {
  describe('App tabs', () => {
    it('should render self and subcomponents', () => {
      const { enzymeWrapper } = setup()

      // there's a div with tabsContainer class
      const tabContainer = enzymeWrapper.find('.tabsContainer');
      expect(tabContainer.length).toBe(1);

      // there's the react Tabs component in that div
      const tabsComponent = tabContainer.find(Tabs);
      expect(tabsComponent.length).toBe(1);

      // and also five react Tab component with proper label
      const tabComponents = tabContainer.find(Tab);
      expect(tabComponents.length).toBe(5);
      expect(tabComponents.at(0).props().label).toBe('Projects');
      expect(tabComponents.at(1).props().label).toBe('Model');
      expect(tabComponents.at(2).props().label).toBe('BOM');
      expect(tabComponents.at(3).props().label).toBe('Drawing');
      expect(tabComponents.at(4).props().label).toBe('Downloads');

      // and the same number of div elements with proper tabContent class
      const tabs = tabContainer.find('.tabContent');
      expect(tabs.length).toBe(5);
    })

  })
})