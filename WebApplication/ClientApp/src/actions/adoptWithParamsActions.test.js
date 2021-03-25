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

import { adoptProjectWithParameters } from './adoptWithParamsActions';

import { actionTypes as uiFlagsActionTypes } from './uiFlagsActions';
import { actionTypes as projectListActionTypes } from './projectListActions';

import signalRConnectionMock from '../test/mockSignalR';

// mock store
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);
const store = mockStore({});

describe('adoptProjectWithParameters workflows for embedded mode ', () => {
    beforeEach(() => { // Runs before each test in the suite
        store.clearActions();
    });

    it('check adoptProjectWithParameters success path', async () => {
        await store.dispatch(adoptProjectWithParameters("url"));
        const projectData = { id: "1", data: "someData" };

        signalRConnectionMock.simulateComplete(projectData, "cloudCrreditStats");

        // check expected store actions
        const actions = store.getActions();
        expect(actions.some(a => (a.type === uiFlagsActionTypes.SHOW_ADOPT_WITH_PROPERTIES_PROGRESS && a.visible === false))).toEqual(true);
        const addProject = actions.find(a => a.type === projectListActionTypes.ADD_PROJECT);
        expect(addProject.newProject).toEqual(projectData);
        const activeProject = actions.find(a => a.type === projectListActionTypes.ACTIVE_PROJECT_UPDATED);
        expect(activeProject.activeProjectId).toEqual(projectData.id);
        const activeTab = actions.find(a => a.type === uiFlagsActionTypes.UPDATE_ACTIVE_TAB_INDEX);
        expect(activeTab.index).toEqual(0);
    });

    it('check adoptProjectWithParameters error path', async () => {
        await store.dispatch(adoptProjectWithParameters("url"));
        signalRConnectionMock.simulateErrorWithReport("jobId", "errorReportLink");

        // check expected store actions
        const actions = store.getActions();
        expect(actions.some(a => (a.type === uiFlagsActionTypes.SHOW_ADOPT_WITH_PROPERTIES_PROGRESS && a.visible === false))).toEqual(true);
        //expect(actions.some(a => (a.type === uiFlagsActionTypes.SHOW_ADOPT_WITH_PARAMS_FAILED && a.visible === true))).toEqual(true);
    });
});