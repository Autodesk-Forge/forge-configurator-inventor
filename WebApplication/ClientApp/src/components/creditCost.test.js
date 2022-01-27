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
import { CreditCost } from './creditCost';

Enzyme.configure({ adapter: new Adapter() });

describe('Show processing stats and cost', () => {
    it('Shows the cost  and time when run FDA was invoked', () => {
        const stats = { credits: 11, processing: 2 };
        const wrapper = shallow(<CreditCost stats={stats} />);
        const texts = wrapper.find('Typography');
        expect(texts.someWhere((t) => t.html().includes('Consumed resources'))).toBeTruthy();
        expect(wrapper.instance().props.stats).toEqual(stats);
    });

    it('Shows the last cost when cached result was used', () => {
        const stats = { credits: 3 };
        const wrapper = shallow(<CreditCost stats={stats} />);
        const texts = wrapper.find('Typography');
        expect(texts.someWhere((t) => t.html().includes('Used cache'))).toBeTruthy();
        expect(wrapper.instance().props.stats).toEqual(stats);
    });
});