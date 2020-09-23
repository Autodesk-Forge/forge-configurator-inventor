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
import { ForgePdfView } from './forgePdfView';

Enzyme.configure({ adapter: new Adapter() });

const loadModelMock = jest.fn();
const unloadModelMock = jest.fn();
const loadExtensionMock = jest.fn();
const viewerFinishMock = jest.fn();
const adskViewingShutdownMock = jest.fn();

class GuiViewer3DMock {
  loadExtension(extId) { loadExtensionMock(extId); }
  loadModel(model) { loadModelMock(model); }
  unloadModel(model) { unloadModelMock(model); }
  start() {}
  finish() { viewerFinishMock(); }
}

const AutodeskMock = {
  Viewing: {
    GuiViewer3D: GuiViewer3DMock,
    Initializer: (_, handleViewerInit) => {
      handleViewerInit();
    },
    shutdown: adskViewingShutdownMock
  }
};

describe('components', () => {
  describe('ForgePdfView', () => {
    const drawingPdf = 'some.pdf';
    const baseProps = { drawingPdf: drawingPdf };

    beforeEach(() => {
        loadModelMock.mockClear();
        unloadModelMock.mockClear();
        loadExtensionMock.mockClear();
        viewerFinishMock.mockClear();
        adskViewingShutdownMock.mockClear();
    });

    it('load gets called when pdf provided', async () => {
      const wrapper = shallow(<ForgePdfView { ...baseProps } />);

      const viewer = wrapper.find('.viewer');
      expect(viewer).toHaveLength(1);
      const script = viewer.find('Script');
      expect(script).toHaveLength(1);

      window.Autodesk = AutodeskMock;
      await script.simulate('load');
      await Promise.resolve(); // waits until all is done

      expect(loadExtensionMock).toHaveBeenCalledWith('Autodesk.PDF');
      expect(loadModelMock).toHaveBeenCalledWith(drawingPdf);
    });

    it('load gets called when pdf changes', async () => {
        const wrapper = shallow(<ForgePdfView { ...baseProps } />);

        window.Autodesk = AutodeskMock;
        const script = wrapper.find('Script');
        await script.simulate('load');
        await Promise.resolve(); // waits until all is done

        const updateProps = { drawingPdf: 'newurl.pdf' };
        wrapper.setProps(updateProps);
        expect(loadModelMock).toHaveBeenCalledTimes(2);
        expect(unloadModelMock).toHaveBeenCalledTimes(1);
    });

    it('returns without loading when pdf is null', async () => {
      const nullProps = { drawingPdf: null };
      const wrapper = shallow(<ForgePdfView { ...nullProps } />);

      window.Autodesk = AutodeskMock;
      const script = wrapper.find('Script');
      await script.simulate('load');
      await Promise.resolve(); // waits until all is done

      expect(loadModelMock).toHaveBeenCalledTimes(0);
    });

    it('unmounts correctly', async () => {
        const wrapper = shallow(<ForgePdfView { ...baseProps } />);

      // preparation: must load the viewer first
         const viewer = wrapper.find('.viewer');
        expect(viewer).toHaveLength(1);
        const script = viewer.find('Script');
        expect(script).toHaveLength(1);
        window.Autodesk = AutodeskMock;
        await script.simulate('load');
        await Promise.resolve(); // waits until all is done

        wrapper.unmount();

        expect(viewerFinishMock).toHaveBeenCalled();
        expect(adskViewingShutdownMock).toHaveBeenCalled();
    });
  });
});