import React from 'react'
import Enzyme, { shallow } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import { ParametersContainer } from './parametersContainer'

Enzyme.configure({ adapter: new Adapter() })

const params = [
    {
        "name": "Length",
        "value": "12000 mm",
        "type": "NYI",
        "units": "mm",
        "allowedValues": []
    },
    {
        "name": "Width",
        "value": "2000 mm",
        "type": "NYI",
        "units": "mm",
        "allowedValues": []
    }
]

const baseProps = {
    activeProject: { parameters: params },
    fetchParameters: () => {}
}

describe('components', () => {
    describe('paramaters constainer', () => {
        it('should propagate data to proper properties', () => {
            const wrapper = shallow(<ParametersContainer {...baseProps} />);
            var objProps = wrapper.props();
            expect(objProps.children.length).toBe(params.length);
            expect(objProps.children[0].props.parameter).toBe(params[0]);
            expect(objProps.children[1].props.parameter).toBe(params[1]);
        })
        it('should start loading projects on mount', () => {
            var fetchParameters = jest.fn();

            const props = {
                ...baseProps,
                fetchParameters
            }

            /*const wrapper = */shallow(<ParametersContainer {...props} />);

            expect(fetchParameters).toBeCalled();
        })
    })
  })
