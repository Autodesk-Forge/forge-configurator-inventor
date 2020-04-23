import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import { TabsContainer } from './tabsContainer'

Enzyme.configure({ adapter: new Adapter() })

const projectList = {
  activeProjectId: '2',
  projects: [
      {
          id: '1',
          label: 'New Project',
          image: 'new_image.png',
          currentHash: 'aaa111'
      },
      {
          id: '2',
          label: 'New Project B',
          image: 'new_image_B.png',
          currentHash: 'bbb222'
      }]
}

const baseProps = {
  projectList,
  fetchProjects: () => {}
}

describe('components', () => {
  describe('App tabs', () => {
    it('CHANGE THE DESCRIPTION ONCE YOU WILL HAVE REAL TEST', () => {
      const enzymeWrapper = shallow(<TabsContainer { ...baseProps } />);
    })
  })
})