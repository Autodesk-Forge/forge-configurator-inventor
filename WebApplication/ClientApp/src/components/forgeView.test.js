import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import { ForgeView } from './forgeView'

Enzyme.configure({ adapter: new Adapter() })

const projectList = {
  activeProjectId: '2',
  projects: [
      {
          id: '1',
          label: 'New Project',
          image: 'new_image.png',
          svf: 'aaa111'
      },
      {
          id: '2',
          label: 'New Project B',
          image: 'new_image_B.png',
          svf: 'bbb222'
      }]
}

const baseProps = {
  projectList,
  fetchProjects: () => {}
}

describe('components', () => {
  describe('forge view', () => {
    it('CHANGE THE DESCRIPTION ONCE YOU WILL HAVE REAL TEST', () => {
      /*const enzymeWrapper = */shallow(<ForgeView { ...baseProps } />);
    })
  })
})