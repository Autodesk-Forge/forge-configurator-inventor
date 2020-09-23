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
import ModalUpdateFailed from './modalUpdateFailed';

Enzyme.configure({ adapter: new Adapter() });

const onCloseMock = jest.fn();
const props = {
    open: true,
    title: "Title",
    label: "Label",
    url: "link",
    onClose: onCloseMock
};

describe('modal update failed dlg', () => {

    beforeEach(() => {
        onCloseMock.mockClear();
    });

    it('check that props are passed correctly', () => {
        const wrapper = shallow(<ModalUpdateFailed {...props} />);
        const modal = wrapper.find('Modal');
        expect(modal.prop('open')).toEqual(props.open);
        expect(modal.prop('title')).toEqual(props.title);

        const labelText = wrapper.find('.modalFailContent Typography');
        expect(labelText.html()).toContain(props.label);

        const log = wrapper.find('.logContainer HyperLink');
        expect(log.prop('href')).toEqual(props.url);
    });

    it('check that it closes on IconButton', () => {
        const wrapper = shallow(<ModalUpdateFailed {...props} />);
        const header = shallow(wrapper.prop('headerChildren'));
        const iconButton = header.find('IconButton');
        iconButton.simulate('click');
        expect(onCloseMock).toBeCalled();
    });

    it('check that it closes on Ok button', () => {
        const wrapper = shallow(<ModalUpdateFailed {...props} />);
        const button = wrapper.find('.modalFailButtonsContainer Button');
        button.simulate('click');
        expect(onCloseMock).toBeCalled();
    });
});
