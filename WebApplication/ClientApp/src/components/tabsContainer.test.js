/**
 * @jest-environment ./src/test/custom-test-env.js
 */

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
import { TabsContainer } from './tabsContainer';

Enzyme.configure({ adapter: new Adapter() });

const updateActivetabIndexMock = jest.fn();

describe('tabsContainer', () => {
    it('handles tab change', () => {
        const wrapper = shallow(<TabsContainer updateActiveTabIndex = {updateActivetabIndexMock} />);
        const tabsWrapper = wrapper.find('Tabs');
        tabsWrapper.simulate('TabChange', 3);
        expect(updateActivetabIndexMock).toHaveBeenCalledWith(3);
    });
});