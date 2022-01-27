/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import { ModalFail } from './modalFail';
Enzyme.configure({ adapter: new Adapter() });

describe('modal update failed', () => {

    it('should show message from props.label', () => {
        const props = {
            title: "modal update failed title",
            label: "project id"
        };

        const wrapper = shallow(<ModalFail {...props} />);
        const wrapperComponent = wrapper.find('.modalFailContent');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(3);
        expect(children[0].props.children.props.children[2]).toBe(props.label);
        expect(children[1]).toBeUndefined(); // href to report
        expect(children[2]).toBeUndefined(); // error message
    });

    it('should show message that props.label is missing', () => {
        const propsNoTitle = { title: null };

        const wrapper = shallow(<ModalFail {...propsNoTitle} />);
        const wrapperComponent = wrapper.find('.modalFailContent');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(3);
        expect(children[0].props.children.props.children[2]).toBe("Missing label.");
        expect(children[1]).toBeUndefined(); // href to report
        expect(children[2]).toBeUndefined(); // error message
    });

    it('should close when Ok button clicked', () => {
        const closeMockFn = jest.fn();
        const props = { onClose: closeMockFn };

        const wrapper = shallow(<ModalFail {...props} />);
        const openButton = wrapper.find({ title: "Ok" });
        expect(openButton.length).toEqual(1);

        openButton.simulate('click');
        expect(closeMockFn).toHaveBeenCalledTimes(1);
    });

    describe('"Report URL" mode', () => {
        it.each([
            { errorType: 1, reportUrl: 'http://example.com' }, // regular URLs are supported
            { errorType: 1, reportUrl: 'ftp://example.com' }, // report url can be anything
            { errorType: 1, reportUrl: 'file:///example.com' }, // report url can be anything
            { errorType: 1, reportUrl: 'blah' }, // report url can be anything
        ])('should link to the reportUrl in the errorData of the props (%s)', (errorData) => {

            const wrapper = shallow(<ModalFail errorData={ errorData } />);

            const wrapperComponent = wrapper.find('.logContainer');
            const children = wrapperComponent.prop('children');

            expect(children.props).toMatchObject({ link: 'Open log file', href: errorData.reportUrl });
            expect(wrapper.find('.errorMessage').exists()).toEqual(false); // error message area is not visible
        });

        // obsolete way to pass report URL, but still supported (to be removed one day)
        it.each([
            'http://example.com', // HTTP
            'https://example.com', // HTTPS
            'HTTPS://example.com', // HTTPS different case
        ])('should link to the reportUrl in the props (%s)', (url) => {

            const wrapper = shallow(<ModalFail errorData={ url } />);

            const wrapperComponent = wrapper.find('.logContainer');
            const children = wrapperComponent.prop('children');

            expect(children.props).toMatchObject({ link: 'Open log file', href: url });
            expect(wrapper.find('.errorMessage').exists()).toEqual(false); // error message area is not visible
        });
    });

    describe('"Error message" mode', () => {

        it.each([
            { errorType: 2, messages: ['blah'] }, // single message
            { errorType: 2, messages: ['blah'], title: 'foo' }, // single message with title
            { errorType: 2, messages: ['blah', 'asdf', 'lkfvbsdf43q4', 'http://foo.com'] }, // multiple messages
            { errorType: 2, messages: ['blah', 'asdf'], title: 'foo' }, // multiple messages with title
        ])('should error message (%s)', (errorData) => {

            const wrapper = shallow(<ModalFail errorData={ errorData } />);

            const errorMessageBlock = wrapper.find('.errorMessage');
            const text = errorMessageBlock.render().text();

            // each message should be shown
            errorData.messages.forEach(m => expect(text.indexOf(m)).not.toEqual(-1));

            // validate title
            const titleBlock = wrapper.find('.errorMessageTitle');
            expect(titleBlock.exists()).toEqual(!! errorData.title);
            expect(wrapper.find('.logContainer').exists()).toEqual(false); // 'Open link' area is not visible
        });

        it.each([
            { errorType: 2 }, // no messages (don't fail)
            { errorType: 2, messages: [] }, // no messages (don't fail)
        ])('should handle no error message (%s)', (errorData) => {

            const wrapper = shallow(<ModalFail errorData={ errorData } />);

            const errorMessageBlock = wrapper.find('.errorMessage');
            expect(errorMessageBlock.exists()).toEqual(false);
            expect(wrapper.find('.logContainer').exists()).toEqual(false); // 'Open link' area is not visible
        });

        // obsolete way to pass error message, but still supported (to be removed one day)
        // sometimes `url` field contains error message. See `ModalFail` implementation for more details
        it.each([
            'The quick brown fox jumps over the lazy dog',
            'httpppp' // should allow url-looking strings
        ])('should detect and show error message (%s)', (message) => {

            const wrapper = shallow(<ModalFail errorData={ message } />);

            const errorMessageBlock = wrapper.find('.errorMessage');
            const text = errorMessageBlock.render().text();

            expect(text.indexOf(message)).not.toEqual(-1);
            expect(wrapper.find('.logContainer').exists()).toEqual(false); // 'Open link' area is not visible
        });
    });

    it('should not fail for unknown error type', () => {

        const errorData = { errorType: 999, status: '1234567890' };
        const wrapper = shallow(<ModalFail errorData={ errorData } />);

        const errorMessageBlock = wrapper.find('.errorMessage');
        const text = errorMessageBlock.render().text();
        expect(text).toMatch(/1234567890/);
        expect(text).toMatch(/Unexpected error/);

        expect(wrapper.find('.logContainer').exists()).toEqual(false); // 'Open link' area is not visible
    });
});
