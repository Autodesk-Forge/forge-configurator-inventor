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
import { App } from './App';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('App', () => {
    it('Test that app will fetch info about showing changed parameters ', () => {
        const fetchShowParametersChanged = jest.fn();
        const detectToken = jest.fn();
        const setEnableEmbeddedMode = jest.fn();

        const props = {
          fetchShowParametersChanged,
          detectToken,
          setEnableEmbeddedMode
        };

        shallow(<App {...props}/>);
        expect(detectToken).toHaveBeenCalled();
        expect(fetchShowParametersChanged).toHaveBeenCalled();
        expect(setEnableEmbeddedMode).toBeCalledWith(false);
    });
  });
});