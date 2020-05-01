import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import { Parameter } from './parameter'

Enzyme.configure({ adapter: new Adapter() })

const param = {
                "name": "Length",
                "value": "1000 mm",
                "type": "NYI",
                "units": "mm",
                "allowedValues": []
}

describe('components', () => {
  describe('parameter', () => {
    it('CHANGE THE DESCRIPTION ONCE YOU WILL HAVE REAL TEST', () => {
        const enzymeWrapper = shallow(<Parameter parameter={param} />);
    })
  })
})