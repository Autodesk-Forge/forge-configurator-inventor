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
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

// prepare mock for Repository module
jest.mock('../Repository');
import mockedRepo from '../Repository';

import { Downloads, downloadColumns } from './downloads';

Enzyme.configure({ adapter: new Adapter() });

const props = {
    activeProject: {
        id: 'foo',
        modelDownloadUrl: 'a/b/c/',
        bomDownloadUrl: 'd/e/f/'
    }
};

describe('Downloads components', () => {

  beforeEach(() => {
    mockedRepo.getAccessToken.mockClear();
  });

  describe('Base Table', () => {
    it('Resizer reduces size', () => {
      const wrapper = shallow(<Downloads { ...props } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );
      expect(bt.prop('width')).toEqual(84);
      expect(bt.prop('height')).toEqual(184);
    });

    it('Row event handlers calls the inner function', () => {
      const wrapper = shallow(<Downloads { ...props } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );
      const jestfce = jest.fn();
      bt.prop('rowEventHandlers').onClick({ rowData: { clickHandler: jestfce }});
      expect(jestfce).toHaveBeenCalledTimes(1);
    });

    it('Base table has expected columns', () => {
      const wrapper = shallow(<Downloads { ...props } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );
      expect(bt.prop('columns')).toEqual(downloadColumns);
    });

    it('Base table has expected data', () => {
      const wrapper = shallow(<Downloads { ...props } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );
      const btdata = bt.prop('data');

      const iam = btdata[0];
      expect(iam.id).toEqual('updatedIam');
      const iamlink = shallow(iam.link);
      expect(iamlink.prop('href')).toEqual(props.activeProject.modelDownloadUrl);
      const stopPropagation = jest.fn();
      iamlink.simulate('click', { stopPropagation });
      expect(stopPropagation).toHaveBeenCalled();
      // iam.clickHandler();

      const rfa = btdata[1];
      expect(rfa.id).toEqual('rfa');
      const rfalink = shallow(rfa.link);
      expect(rfalink.prop('href')).toEqual('');
      const preventDefault = jest.fn();
      rfalink.simulate('click', { preventDefault });
      expect(preventDefault).toHaveBeenCalled();
    });

    it.each([
      [ {}, 0], // empty project info => no download links
      [ { id: 'foo' }, 1], // no URLs => only RFA link if available (extreme case for code coverage)
      [ props.activeProject, 2 ], // no `isAssembly` field. Assuming - no BOM is available (extreme case for code coverage)
      [ { ...props.activeProject, isAssembly: false }, 2 ], // no BOM available for parts
      [ { ...props.activeProject, isAssembly: true }, 3 ], // BOM download is expected
      [ { ...props.activeProject, isAssembly: true, hasDrawing: true }, 4] // BOM and Drawings are expected
    ])('Base table renders expected count of links and icons - %# case',
      (project, count) => {

        const props = { activeProject: project };
        const wrapper = mount(<Downloads { ...props } />);
        const as = wrapper.find('AutoResizer');
        const bt = as.renderProp('children')( {width: 100, height: 200} );

        const icons = bt.find('Icon');
        const hyperlinks = bt.find('a');

        expect(icons.length).toEqual(count);
        expect(hyperlinks.length).toEqual(count);
      });

    it('Base table renders NO links and icons when projects are empty', () => {
      // simulate activeProject (getActiveProject) like we have in these two scenarios:
      // 1) don't have initialized projects yet
      // 2) user don't have any projects
      const noActiveProjectProps = { activeProject: {} };
      const wrapper = mount(<Downloads { ...noActiveProjectProps } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );
      const icons = bt.find('Icon');
      const hyperlinks = bt.find('a');
      expect(icons.length).toEqual(0);
      expect(hyperlinks.length).toEqual(0);
    });

    it('should inject token for download URLs', () => {

      const fakeToken = '1234567890';
      mockedRepo.getAccessToken.mockReturnValue(fakeToken);

      const wrapper = shallow(<Downloads { ...props } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );
      const btdata = bt.prop('data');
      const iam = btdata[0];
      const iamlink = shallow(iam.link);
      expect(iamlink.prop('href').endsWith(fakeToken)).toEqual(true);
    });
  });

  describe('Popup windows', () => {
    describe('RFA', () => {
      const showModalProgressMock = jest.fn();
      const showFailedMock = jest.fn();
      const downloadUrl='downloadUrl';
      const failedReportUrl='failedReportUrl';
      const rfaProps = { ...props, downloadProgressShowing:true, downloadUrl: downloadUrl, showDownloadProgress: showModalProgressMock, downloadProgressTitle: 'RFA' };
      const rfaFailedProps = { ...props, downloadFailedShowing:true, reportUrl:failedReportUrl, showDownloadFailed: showFailedMock, downloadProgressTitle: 'RFA' };

      beforeEach(() => {
        showModalProgressMock.mockClear();
        showFailedMock.mockClear();
      });

      it('Shows progress dialog', () => {
        const wrapper = shallow(<Downloads { ...rfaProps} />);
        const dlg = wrapper.find('ModalDownloadProgress');
        expect(dlg.prop('label')).toEqual(props.activeProject.id);
        expect(dlg.prop('title')).toContain('RFA');
        expect(dlg.prop('url')).toEqual(downloadUrl);
      });

      it('Handles progress dialog Close click', () => {
        const wrapper = shallow(<Downloads { ...rfaProps} />);
        const dlg = wrapper.find('ModalDownloadProgress');

        dlg.simulate('close');
        expect(showModalProgressMock).toBeCalledWith(false);
      });

      it('Shows failed dialog', () => {
        const wrapper = shallow(<Downloads { ...rfaFailedProps} />);
        const dlg = wrapper.find('ModalFail');
        expect(dlg.prop('label')).toEqual(props.activeProject.id);
        expect(dlg.prop('title')).toContain('RFA');
        expect(dlg.prop('url')).toEqual(failedReportUrl);
      });

      it('Handles failed dialog Close click', () => {
        const wrapper = shallow(<Downloads { ...rfaFailedProps} />);
        const dlg = wrapper.find('ModalFail');

        dlg.simulate('close');
        expect(showFailedMock).toBeCalledWith(false);
      });
    });

    describe('Drawings', () => { // TODO: some unit tests are not required anymore, since the same dialogs used for different downloads
      const showModalProgressMock = jest.fn();
      const showFailedMock = jest.fn();
      const downloadUrl='downloadUrl';
      const failedReportUrl='failedReportUrl';
      const drwProps = { ...props, downloadProgressShowing:true, downloadUrl, showDownloadProgress:showModalProgressMock, downloadProgressTitle: 'Drawings'};
      const drwFailedProps = { ...props, downloadFailedShowing:true, reportUrl:failedReportUrl, showDownloadFailed:showFailedMock, downloadProgressTitle: 'Drawings'};

      beforeEach(() => {
        showModalProgressMock.mockClear();
        showFailedMock.mockClear();
      });

      it('Shows progress dialog', () => {
        const wrapper = shallow(<Downloads { ...drwProps} />);
        const dlg = wrapper.find('ModalDownloadProgress');
        expect(dlg.prop('label')).toEqual(props.activeProject.id);
        expect(dlg.prop('title')).toContain('Drawings');
        expect(dlg.prop('url')).toEqual(downloadUrl);
      });

      it('Handles progress dialog Close click', () => {
        const wrapper = shallow(<Downloads { ...drwProps} />);
        const dlg = wrapper.find('ModalDownloadProgress');

        dlg.simulate('close');
        expect(showModalProgressMock).toBeCalledWith(false);
      });

      it('Shows failed dialog', () => {
        const wrapper = shallow(<Downloads { ...drwFailedProps} />);
        const dlg = wrapper.find('ModalFail');
        expect(dlg.prop('label')).toEqual(props.activeProject.id);
        expect(dlg.prop('title')).toContain('Drawings');
        expect(dlg.prop('url')).toEqual(failedReportUrl);
      });

      it('Handles failed dialog Close click', () => {
        const wrapper = shallow(<Downloads { ...drwFailedProps} />);
        const dlg = wrapper.find('ModalFail');

        dlg.simulate('close');
        expect(showFailedMock).toBeCalledWith(false);
      });
    });
  });
});
