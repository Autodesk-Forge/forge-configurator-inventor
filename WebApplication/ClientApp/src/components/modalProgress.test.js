import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ModalProgress } from './modalProgress';

Enzyme.configure({ adapter: new Adapter() });

const props = {
    title: "modal progress dialog title",
    label: "name of file in progress",
    icon: "Archive.svg"
};

describe('modal progress ', () => {

    it('should show message from props.label', () => {

        const wrapper = shallow(<ModalProgress {...props} />);

        const wrapperComponent = wrapper.find('.modalAction');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0]).toBe(props.label);
    });

    it('should show message that props.label is missing', () => {

        const propsNoTitle = { title: null };

        const wrapper = shallow(<ModalProgress {...propsNoTitle} />);

        const wrapperComponent = wrapper.find('.modalAction');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0]).toBe("Missing label.");
    });
});
