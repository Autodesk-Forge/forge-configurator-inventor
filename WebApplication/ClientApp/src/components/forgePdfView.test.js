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
const hideModelMock = jest.fn();
const showModelMock = jest.fn();
const loadExtensionMock = jest.fn();
const viewerFinishMock = jest.fn();
const adskViewingShutdownMock = jest.fn();

let allModelsData = [];

class ModelMock {

  constructor(url)
  {
    this.data = { urn: url };
  }

  getData() { return this.data; }
}

class GuiViewer3DMock {
  loadExtension(extId) { loadExtensionMock(extId); }
  loadModel(modelUrl, options) {
    loadModelMock(modelUrl, options);
    allModelsData.push(new ModelMock(modelUrl));
  }
  hideModel(model) { hideModelMock(model); }
  showModel(model) { showModelMock(model); }
  getAllModels() { return allModelsData; }
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
        hideModelMock.mockClear();
        showModelMock.mockClear();
        loadExtensionMock.mockClear();
        viewerFinishMock.mockClear();
        adskViewingShutdownMock.mockClear();

        allModelsData = [];
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
      expect(loadModelMock).toHaveBeenCalledWith( drawingPdf, {page: 1});
    });

    it('load/hide/show gets called when pdf changes', async () => {
        const wrapper = shallow(<ForgePdfView { ...baseProps } />);

        window.Autodesk = AutodeskMock;
        const script = wrapper.find('Script');
        await script.simulate('load');
        await Promise.resolve(); // waits until all is done

        const newUrl = 'newurl.pdf';
        let updateProps = { drawingPdf: newUrl };
        wrapper.setProps(updateProps);
        expect(loadModelMock).toHaveBeenCalledTimes(2);
        expect(hideModelMock).toHaveBeenCalledTimes(1);
        // expect that the first pdf will be hide
        expect(hideModelMock).toHaveBeenCalledWith({"data": {"urn": drawingPdf}});

        hideModelMock.mockClear();

        // return back to startup url
        updateProps = { drawingPdf: drawingPdf };
        wrapper.setProps(updateProps);

        // expect that the second pdf will be hide when selecting back the first one
        expect(hideModelMock).toHaveBeenCalledTimes(1);
        expect(hideModelMock).toHaveBeenCalledWith({"data": {"urn": newUrl}});

        // original pdf will be displayed
        expect(showModelMock).toHaveBeenCalledTimes(1);
        expect(showModelMock).toHaveBeenCalledWith({"data": {"urn": drawingPdf}});
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