import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ModalProgressUpload } from './modalProgressUpload';

Enzyme.configure({ adapter: new Adapter() });

describe('modal progress ', () => {

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
