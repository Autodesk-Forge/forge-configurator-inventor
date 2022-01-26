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
import { ModalProgressUpload } from './modalProgressUpload';

Enzyme.configure({ adapter: new Adapter() });

describe('modal progress', () => {

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

    it('shows the Complete24 icon when done without warning', () => {
        const props = { isDone: () => { return true;}, warningMsg: undefined };
        const wrapper = shallow(<ModalProgressUpload {...props} />);
        const header = shallow(wrapper.prop('headerChildren'));
        const icon = header.find('Complete24');
        expect(icon.length).toEqual(1);
    });

    it('shows the OTHER icon when done with warning', () => {
        const props = { isDone: () => { return true;}, warningMsg: 'Unsupported plugin!' };
        const wrapper = shallow(<ModalProgressUpload {...props} />);
        const header = shallow(wrapper.prop('headerChildren'));
        const icon = header.find('#warningIcon');
        expect(icon.length).toEqual(1);
    });

    it('shows the warning when one is passed in', () => {
        const warningMsg = 'Unsupported plugin!';
        const props = { isDone: () => { return true;}, warningMsg: warningMsg };
        const wrapper = shallow(<ModalProgressUpload {...props} />);
        const msg = wrapper.find('#warningMsg');
        expect(msg.length).toEqual(1);
        expect(msg.html()).toContain(warningMsg);
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
