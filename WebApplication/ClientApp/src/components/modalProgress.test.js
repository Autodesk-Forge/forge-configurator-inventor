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
});
