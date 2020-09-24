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

// prepare mock for Repository module
jest.mock('../Repository');
import mockedRepo from '../Repository';

import { DrawingsContainer, drawingColumns } from './drawingsContainer';

Enzyme.configure({ adapter: new Adapter() });

const downloadUrl = "downloadUrl";
const errorData = { errorType: 1, reportUrl: "reportUrl"};
const drawingsList = [ "drawing0", "drawing1", "drawing2" ];

const getDownloadLinkMock = jest.fn();
const fetchDrawingsListMock = jest.fn();
const updateActiveDrawingMock = jest.fn();
const showDownloadProgressMock = jest.fn();
const showDownloadFailedMock = jest.fn();

const props = {
    activeProject: {
        id: 'foo',
        hash: '123'
    },
    downloadProgressShowing: false,
    downloadProgressTitle: "",
    downloadFailedShowing: false,
    downloadUrl: downloadUrl,
    errorData: errorData,
    activeDrawing: drawingsList[1],
    drawingsList: drawingsList,
    getDownloadLink: getDownloadLinkMock,
    fetchDrawingsList: fetchDrawingsListMock,
    updateActiveDrawing: updateActiveDrawingMock,
    showDownloadProgress: showDownloadProgressMock,
    showDownloadFailed: showDownloadFailedMock
};

describe('DrawingsContainer component', () => {

    beforeEach(() => {
        mockedRepo.getAccessToken.mockClear();
        getDownloadLinkMock.mockClear();
        fetchDrawingsListMock.mockClear();
        updateActiveDrawingMock.mockClear();
        showDownloadProgressMock.mockClear();
        showDownloadFailedMock.mockClear();
    });

    describe('Base Table', () => {
        it('Resizer reduces size', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            expect(bt.prop('width')).toEqual(100);
            expect(bt.prop('height')).toEqual(184);
        });

        it('Row event handlers calls the inner function - and does nothing with activeDrawing id ', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            bt.prop('rowEventHandlers').onClick({ rowData: { id: drawingsList[1] }});
            expect(updateActiveDrawingMock).toHaveBeenCalledTimes(0);
        });

        it('Row event handlers calls the inner function - and calls update with other than activeDrawing id ', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            bt.prop('rowEventHandlers').onClick({ rowData: { id: drawingsList[2] }});
            expect(updateActiveDrawingMock).toHaveBeenCalledWith(drawingsList[2]);
        });

        it('Base table has expected columns', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            expect(bt.prop('columns')).toEqual(drawingColumns);
        });

        it('Base table has expected data', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            const btdata = bt.prop('data');

            expect(btdata.length).toEqual(drawingsList.length);
            btdata.forEach( (row, index) => {
                expect(row.id).toEqual(drawingsList[index]);
                expect(row.label).toEqual(drawingsList[index]);
            });
        });

        it('Base table sets row class name for activeDrawing', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            expect(bt.prop('rowClassName')({ rowData: { id: drawingsList[1] }})).toEqual('drawing-selected');
        });

        it('Base table does not set row class name for non-activeDrawing', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const as = wrapper.find('AutoResizer');
            const bt = as.renderProp('children')( {width: 100, height: 200} );
            expect(bt.prop('rowClassName')({ rowData: { id: drawingsList[2] }})).toEqual('');
        });
    });

    describe('Downloads and updates', () => {
        it('Downloads correct PDF', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);
            const exportButton = wrapper.find('.buttonsContainer Button');
            exportButton.simulate('click');
            expect(getDownloadLinkMock).toHaveBeenCalledWith('CreateDrawingPdfJob', props.activeProject.id, props.activeProject.hash, 'Preparing Drawing PDF', props.activeDrawing);
        });

        it('Fetches new drawing on component update', () => {
            const wrapper = shallow(<DrawingsContainer { ...props } />);

            const newActiveProject = { id: 'new Foo', hash: '345' };
            const updatedProps = { ...props, activeProject: newActiveProject };
            wrapper.setProps(updatedProps);

            expect(fetchDrawingsListMock).toHaveBeenCalledWith(newActiveProject);
        });
    });

    describe('Popup windows', () => {
        const dlgTitle = "The Title";
        const okProps = { ...props, downloadProgressShowing:true, downloadUrl: downloadUrl, showDownloadProgress: showDownloadProgressMock, downloadProgressTitle: dlgTitle };
        const failedProps = { ...props, downloadFailedShowing:true, errorData, showDownloadFailed: showDownloadFailedMock, downloadProgressTitle: dlgTitle };

        it('Shows progress dialog', () => {
            const wrapper = shallow(<DrawingsContainer { ...okProps} />);
            const dlg = wrapper.find('ModalDownloadProgress');
            expect(dlg.prop('label')).toEqual(props.activeProject.id);
            expect(dlg.prop('title')).toContain(dlgTitle);
            expect(dlg.prop('url')).toEqual(downloadUrl);
        });

        it('Handles progress dialog Close click', () => {
            const wrapper = shallow(<DrawingsContainer { ...okProps} />);
            const dlg = wrapper.find('ModalDownloadProgress');

            dlg.simulate('close');
            expect(showDownloadProgressMock).toBeCalledWith(false);
        });

        it('Shows failed dialog', () => {
            const wrapper = shallow(<DrawingsContainer { ...failedProps} />);
            const dlg = wrapper.find('ModalFail');
            expect(dlg.prop('label')).toEqual(props.activeProject.id);
            expect(dlg.prop('title')).toContain(dlgTitle);
            expect(dlg.prop('errorData')).toEqual(errorData);
        });

        it('Handles failed dialog Close click', () => {
            const wrapper = shallow(<DrawingsContainer { ...failedProps} />);
            const dlg = wrapper.find('ModalFail');

            dlg.simulate('close');
            expect(showDownloadFailedMock).toBeCalledWith(false);
        });
    });
});