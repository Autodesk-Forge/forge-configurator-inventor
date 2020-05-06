import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ParametersContainer } from './parametersContainer';

Enzyme.configure({ adapter: new Adapter() });

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
];

const baseProps = {
    activeProject: { updateParameters: params },
    fetchParameters: () => {}
};

describe('components', () => {
    describe('parameters constainer', () => {
        it('should propagate data to proper properties', () => {
            const wrapper = shallow(<ParametersContainer {...baseProps} />);
            var wrapperComponent = wrapper.find('.parameters');
            var children = wrapperComponent.prop('children');
            expect(children.length).toBe(params.length);
            expect(children[0].props.parameter).toBe(params[0]);
            expect(children[1].props.parameter).toBe(params[1]);
        });
    });
});
