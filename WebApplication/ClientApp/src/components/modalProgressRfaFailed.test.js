import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ModalProgressRfaFailed } from './modalProgressRfaFailed';
Enzyme.configure({ adapter: new Adapter() });

describe('modal update failed ', () => {

    it('should show message from props.label', () => {
        const props = {
            title: "modal update failed title",
            label: "project id"
        };

        const wrapper = shallow(<ModalProgressRfaFailed {...props} />);
        const wrapperComponent = wrapper.find('.modalProgressRFAFailedContent');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props.children.props.children[2]).toBe(props.label);
    });

    it('should show message that props.label is missing', () => {
        const propsNoTitle = { title: null };

        const wrapper = shallow(<ModalProgressRfaFailed {...propsNoTitle} />);
        const wrapperComponent = wrapper.find('.modalProgressRFAFailedContent');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props.children.props.children[2]).toBe("Missing label.");
    });

    it('should close when Ok button clicked', () => {
        const closeMockFn = jest.fn();
        const props = { onClose: closeMockFn };

        const wrapper = shallow(<ModalProgressRfaFailed {...props} />);
        const okButton = wrapper.find({ title: "Ok" });
        expect(okButton.length).toEqual(1);

        okButton.simulate('click');
        expect(closeMockFn).toHaveBeenCalledTimes(1);
    });

    it('should link to the reportUrl in the props', () => {
        const props = { url: 'http://example.com' };

        const wrapper = shallow(<ModalProgressRfaFailed {...props} />);
        const wrapperComponent = wrapper.find('.logContainer');
        const children = wrapperComponent.prop('children');

        expect(children.props.link).toBe('Open log file');
        expect(children.props.href).toBe(props.url);
    });
});
