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
import Adapter from 'enzyme-adapter-react-16';
import { ModalFail } from './modalFail';
Enzyme.configure({ adapter: new Adapter() });

describe('modal update failed ', () => {

    it('should show message from props.label', () => {
        const props = {
            title: "modal update failed title",
            label: "project id"
        };

        const wrapper = shallow(<ModalFail {...props} />);
        const wrapperComponent = wrapper.find('.modalFailContent');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props.children.props.children[2]).toBe(props.label);
    });

    it('should show message that props.label is missing', () => {
        const propsNoTitle = { title: null };

        const wrapper = shallow(<ModalFail {...propsNoTitle} />);
        const wrapperComponent = wrapper.find('.modalFailContent');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props.children.props.children[2]).toBe("Missing label.");
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

    it('should link to the reportUrl in the props', () => {
        const props = { url: 'http://example.com' };

        const wrapper = shallow(<ModalFail {...props} />);
        const wrapperComponent = wrapper.find('.logContainer');
        const children = wrapperComponent.prop('children');

        expect(children.props.link).toBe('Open log file');
        expect(children.props.href).toBe(props.url);
    });
});
