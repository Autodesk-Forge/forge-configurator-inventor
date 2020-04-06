import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import TabsContainer from './tabsContainer'

Enzyme.configure({ adapter: new Adapter() })

describe('components', () => {
  describe('App tabs', () => {
    it('CHANGE THE DESCRIPTION ONCE YOU WILL HAVE REAL TEST', () => {
      const { enzymeWrapper } = shallow(<TabsContainer />)
    })

  })
})