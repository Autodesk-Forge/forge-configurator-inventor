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

import actionTypes, { fetchBom } from './bomActions';

// the test based on https://redux.js.org/recipes/writing-tests#async-action-creators

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';

import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

// set expected value for the mock
const loadBomMock = repoInstance.loadBom;
const newBOM = {
    columns: [ {label: "Part No"}, {label: "Desc"} ],
    data: [ [ "SM22", "M22 screw"], [ "BM22", "M22 bolt"] ]
};

describe('load BOM', () => {

    let store;

    beforeEach(() => {

        loadBomMock.mockClear();
        loadBomMock.mockResolvedValue(newBOM);

        // prepare empty 'updated parameters' data
        const state = {};

        store = mockStore(state);
        store.getState = () => state;
    });

    it('updates the bom state after fetch BOM', async () => {
        const projectId = 'projectA';
        await store.dispatch(fetchBom(projectId));
        expect(loadBomMock).toBeCalledWith(projectId);

        // check expected store actions
        const actions = store.getActions();
        const updateAction = actions.find(a => a.type === actionTypes.BOM_UPDATED);
        expect(updateAction.projectId).toEqual(projectId);
        expect(updateAction.bomData).toEqual(newBOM);
    });
});