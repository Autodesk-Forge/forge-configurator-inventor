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
import { ModalProgress } from './modalProgress';

Enzyme.configure({ adapter: new Adapter() });

describe('modal progress', () => {
    it('should show default title when in progress', () => {
        const title = 'a title';
        const props = {
            title: title
        };
        const wrapper = shallow(<ModalProgress { ...props } />);
        expect(wrapper.prop('title')).toEqual(title);
    });

    it('should show Done title when the progress is done, which is indicated by valid stats in props', () => {
        const title = 'a title';
        const doneTitle = 'done title';
        const stats = { credits: 3 };
        const props = {
            title: title,
            doneTitle: doneTitle,
            stats: stats
        };
        const wrapper = shallow(<ModalProgress { ...props } />);
        expect(wrapper.prop('title')).toEqual(doneTitle);
    });

    it('should show default title when the progress is done, but no Done title is provided', () => {
        const title = 'a title';
        const stats = { credits: 3 };
        const props = {
            title: title,
            stats: stats
        };
        const wrapper = shallow(<ModalProgress { ...props } />);
        expect(wrapper.prop('title')).toEqual(title);
    });

    it('should show warningMsg when done and message is present', () => {
        const warningMsg = 'Unsupported plugin!';
        const props = {
            title: 'a title',
            doneTitle: 'done title',
            stats: { credits: 3 },
            warningMsg : warningMsg
        };
        const wrapper = shallow(<ModalProgress { ...props } />);
        const msg = wrapper.find('#warningMsg');
        expect(msg.length).toEqual(1);
        expect(msg.html()).toContain(warningMsg);
    });

    it('should show warning Icon when done and warningMsg is present', () => {
        const warningMsg = 'Unsupported plugin!';
        const props = {
            title: 'a title',
            doneTitle: 'done title',
            stats: { credits: 3 },
            warningMsg : warningMsg
        };
        const wrapper = shallow(<ModalProgress { ...props } />);
        const header = shallow(wrapper.prop('headerChildren'));
        const icon = header.find('#warningIcon');
        expect(icon.length).toEqual(1);
    });

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

    it('should call method provided for Done button', () => {
        const doneHandlerMock = jest.fn();
        const props = {
            stats: {},
            onClose: doneHandlerMock
        };

        const wrapper = shallow(<ModalProgress {...props} />);
        const doneBtn = wrapper.find('Button');
        doneBtn.simulate('click');
        expect(doneHandlerMock).toHaveBeenCalled();
    });
});
