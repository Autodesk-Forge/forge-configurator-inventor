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
        activeProject: { id: "1" },
        drawingPdf: ""
      };

      const wrapper = shallow(<Drawing {...props}/>);
      const wrapperComponent = wrapper.find('.drawingEmptyText');
      expect(wrapperComponent.length).toEqual(1);
      const children = wrapperComponent.prop('children');
      expect(children).toEqual("You don't have any drawings in package.");
    });

  it('Page has expected text when package is not assembly', () => {
      const props = {
        activeProject: { id: "1", isAssembly: false }
      };

      const wrapper = shallow(<Drawing {...props}/>);
      const wrapperComponent = wrapper.find('.drawingEmptyText');
      expect(wrapperComponent.length).toEqual(1);
      const children = wrapperComponent.prop('children');
      expect(children).toEqual("You don't have any drawings in package.");
    });

  it('check that fetching of drawing is called when package is assembly', () => {
      const fetchDrawingMock = jest.fn();
      const props = {
        activeProject: { id: "1", isAssembly: true, hasDrawing: true },
        drawingPdf: null, // initialize fetch
        fetchDrawing: fetchDrawingMock
      };

      shallow(<Drawing {...props}/>);
      expect(fetchDrawingMock).toHaveBeenCalledTimes(1);
      expect(fetchDrawingMock).toHaveBeenCalledWith(props.activeProject);
    });

  it('check that fetching of drawing is not called when package is not assembly', () => {
      const fetchDrawingMock = jest.fn();
      const props = {
        activeProject: { id: "1", isAssembly: false },
        drawingPdf: null, // initialize fetch
        fetchDrawing: fetchDrawingMock
      };

      shallow(<Drawing {...props}/>);
      expect(fetchDrawingMock).toHaveBeenCalledTimes(0);
    });

    it('check fetching of drawing when active project changes and the new has no drawing url', () => {
      const fetchDrawingMock = jest.fn();
      const props = {
        activeProject: { id: "1", isAssembly: true, hasDrawing: true },
        drawingPdf: 'a link',
        fetchDrawing: fetchDrawingMock
      };

      const wrapper = shallow(<Drawing {...props}/>);
      expect(fetchDrawingMock).toHaveBeenCalledTimes(0); // already had a link, should not fetch

      const updateProps = {
        activeProject: { id: "2", isAssembly: true, hasDrawing: true },
        drawingPdf: null,
        fetchDrawing: fetchDrawingMock
      };
      wrapper.setProps(updateProps);
      expect(fetchDrawingMock).toHaveBeenCalledTimes(1);
      expect(fetchDrawingMock).toHaveBeenCalledWith(updateProps.activeProject);
    });
});
