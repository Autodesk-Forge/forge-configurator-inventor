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

import Enzyme from 'enzyme';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import * as downloadActions from './downloadActions';
import { actionTypes as uiFlagsActionTypes } from './uiFlagsActions';

const aLink = 'https://some.link';
const tokenMock = 'theToken';
const fullLink = `${aLink}/${tokenMock}`;
const noTokenLink = `${aLink}`;
const errorReportLink = 'https://error.link';
const jobId = 'job1';
const theStats = { credits: 1 };

import signalRConnectionMock from '../test/mockSignalR';

import repoInstance from '../Repository';
repoInstance.getAccessToken = function() {
    return tokenMock;
};

Enzyme.configure({ adapter: new Adapter() });

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

const mockState = {
  uiFlags: {
  }
};
const store = mockStore(mockState);

describe('downloadActions', () => {
    beforeEach(() => { // Runs before each test in the suite
        store.clearActions();
    });

    describe('DownloadLink', () => {
        it('check getDownloadLink onComplete action', async () => {
            await store.dispatch(downloadActions.getDownloadLink("Method", "ProjectId", "hash", "title"));
            // simulate conection.onComplete(rfaLink);
            signalRConnectionMock.simulateComplete(aLink, theStats);

            // check expected store actions
            const actions = store.getActions();
            const linkAction = actions.find(a => a.type === uiFlagsActionTypes.SET_DOWNLOAD_LINK);
            expect(linkAction.url).toEqual(fullLink);
            const statsAction = actions.find(a => a.type === uiFlagsActionTypes.SET_STATS);
            expect(statsAction.stats).toEqual(theStats);
        });

        it('check getDownloadLink onError action', async () => {
            await store.dispatch(downloadActions.getDownloadLink("Method", "ProjectId", "hash", "title"));
            signalRConnectionMock.simulateErrorWithReport(jobId, errorReportLink);

            // check expected store actions
            const actions = store.getActions();
            // there are two SET_REPORT_URL actions in the list. The first one come from job start and is called with null to clear old data...
            expect(actions.some(a => (a.type === uiFlagsActionTypes.SET_ERROR_DATA && a.errorData?.reportUrl === errorReportLink))).toEqual(true);
            expect(actions.some(a => a.type === uiFlagsActionTypes.SHOW_DOWNLOAD_FAILED)).toEqual(true);
        });
    });

    describe('Drawing', () => {
        it('check fetchDrawing action', async () => {
            await store.dispatch(downloadActions.fetchDrawing({ id: "ProjectId" }, "DrawingKey"));
            signalRConnectionMock.simulateComplete(aLink, theStats);

            // check expected store actions
            const actions = store.getActions();
            const linkAction = actions.find(a => a.type === uiFlagsActionTypes.SET_DRAWING_URL);
            expect(linkAction.url).toEqual(noTokenLink);

            // check that stats is preset
            expect(actions.some(a => a.type === uiFlagsActionTypes.SET_STATS && a.stats === theStats)).toBeTruthy();
        });

        it('check fetchDrawing error handling', async () => {
            await store.dispatch(downloadActions.fetchDrawing({ id: "ProjectId" }, "DrawingKey"));
            signalRConnectionMock.simulateErrorWithReport(jobId,errorReportLink);

            // check expected store actions
            const actions = store.getActions();
            expect(actions.some(a => a.type === uiFlagsActionTypes.SHOW_DRAWING_PROGRESS)).toEqual(true);
        });
    });
});