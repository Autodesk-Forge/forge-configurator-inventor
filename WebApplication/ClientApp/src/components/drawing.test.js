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
import {Drawing} from './drawing';

Enzyme.configure({ adapter: new Adapter() });

describe('Drawings', () => {
  it('Page has expected text when no drawings available', () => {
      const props = {
        activeProject: { drawing: null }
      };

      const wrapper = shallow(<Drawing {...props}/>);
      const wrapperComponent = wrapper.find('.drawingEmptyText');
      expect(wrapperComponent.length).toEqual(1);
      const children = wrapperComponent.prop('children');
      expect(children).toEqual("You don't have any drawings in package.");
    });
});
