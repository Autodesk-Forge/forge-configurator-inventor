import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ModalProgress } from './modalProgress';
import { ModalProgressRfa } from './modalProgressRfa';
import { ModalProgressUpload } from './modalProgressUpload';

Enzyme.configure({ adapter: new Adapter() });

describe('modal progress ', () => {

    it('should show message from props.label', () => {

        const props = {
            title: "modal progress dialog title",
            label: "name of file in progress",
            icon: "Archive.svg"
        };

        const wrapper = shallow(<ModalProgress {...props} />);

        const wrapperComponent = wrapper.find('.modalAction');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props['children']).toBe(props.label);
    });

    it('should show message that props.label is missing', () => {

        const propsNoTitle = { title: null };

        const wrapper = shallow(<ModalProgress {...propsNoTitle} />);

        const wrapperComponent = wrapper.find('.modalAction');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props['children']).toBe("Missing label.");
    });

    it('check Done button when specified download url', () => {

        const props = { url: "someUrl" };

        const wrapper = shallow(<ModalProgressRfa {...props} />);

        const button = wrapper.find('Button');
        expect(button.prop('title')).toBe('Done');
    });

    it('check that here is NO button available when not used download url', () => {

        const props = { url: null };

        const wrapper = shallow(<ModalProgressRfa {...props} />);

        const button = wrapper.find('Button');
        expect(button.length).toBe(0);
    });

    it('check that here is NO button available when is not update progress set to done', () => {

        const props = { isDone: () => { return false;} };

        const wrapper = shallow(<ModalProgressUpload {...props} />);

        const button = wrapper.find('Button');
        expect(button.length).toBe(0);
    });

    it('check that here are TWO buttons available when is update progress set to done', () => {

        const props = { isDone: () => { return true;} };

        const wrapper = shallow(<ModalProgressUpload {...props} />);

        const buttons = wrapper.find('Button');
        expect(buttons.length).toBe(2);
    });

    it('check OPEN button click', () => {
        const openMockFn = jest.fn();
        const props = { isDone: () => { return true;},
                        onOpen: openMockFn };

        const wrapper = shallow(<ModalProgressUpload {...props} />);

        const openButton = wrapper.find({ title: "Open"});
        expect(openButton.length).toEqual(1);

        openButton.simulate('click');
        expect(openMockFn).toHaveBeenCalledTimes(1);
    });
});
