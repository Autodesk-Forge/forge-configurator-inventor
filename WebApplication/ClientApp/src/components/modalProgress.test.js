import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ModalProgress } from './modalProgress';

Enzyme.configure({ adapter: new Adapter() });

const projectId = "test_project_id";
const props = {
    activeProject: { id: projectId }
};

describe('modal progress ', () => {

    it('should show message with activeProject.id', () => {

        const wrapper = shallow(<ModalProgress {...props} />);

        const wrapperComponent = wrapper.find('.modalAction');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0]).toBe(projectId);
    });

    it('should show message that active project name is missing', () => {

        const propsNoId = { activeProject: { id: null }};

        const wrapper = shallow(<ModalProgress {...propsNoId} />);

        const wrapperComponent = wrapper.find('.modalAction');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0]).toBe("Missing active project name.");
    });
});
