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

const fetchMock = jest.fn();
const projectId = 1;

const emptyProps = {
    activeProject: { id: projectId },
    fetchParameters: fetchMock
};

describe('parameters container', () => {

    beforeEach(() => {
        fetchMock.mockClear();
    });

    it('verify fetchParameters called immediatelly when component rendered', () => {
        shallow(<ParametersContainer {...emptyProps} />);
        expect(fetchMock).toHaveBeenCalledWith(projectId);
    });

    it('verify fetchParameters called immediatelly when active projecte id was changed', () => {
        const noProj = {
            activeProject: {},
            fetchParameters: fetchMock
        };

        const wrapper = shallow(<ParametersContainer {...noProj} />);
        fetchMock.mockClear();

        const updatedProps = Object.assign(noProj, { activeProject: { id: 2 } });
        wrapper.setProps(updatedProps);
        expect(fetchMock).toHaveBeenCalledWith(2);
    });

    it('should show special message for empty parameters', () => {

        const wrapper = shallow(<ParametersContainer {...emptyProps} />);
        const wrapperComponent = wrapper.find('.parameters');
        const content = wrapperComponent.prop('children');
        expect(content).toEqual("No parameters");
    });

    it('should propagate data to proper properties', () => {

        // prefill props with properties
        const props = Object.assign({ projectUpdateParameters: params }, emptyProps);

        const wrapper = shallow(<ParametersContainer {...props} />);
        const wrapperComponent = wrapper.find('.parameters');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props.parameter).toBe(params[0]);
        expect(children[1].props.parameter).toBe(params[1]);
    });
});
