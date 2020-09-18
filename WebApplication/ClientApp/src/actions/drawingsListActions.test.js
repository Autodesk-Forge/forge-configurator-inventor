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

import { fetchDrawingsList } from './drawingsListActions';
import { actionTypes as uiFlagsActionTypes } from './uiFlagsActions';

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
const loadDrawingsListMock = repoInstance.loadDrawingsList;

const project = { id: 'project1', drawingsListUrl: 'url' };
const drawingsList = [ '1', '3' ];

describe('fetch Drawing List', () => {

    let store;

    beforeEach(() => {

        loadDrawingsListMock.mockClear();
        loadDrawingsListMock.mockResolvedValue(drawingsList);

        // prepare empty 'updated parameters' data
        const state = {};

        store = mockStore(state);
        store.getState = () => state;
    });

    describe('success', () => {
        it('gets and stores drawings list on fetch', async () => {

            await store.dispatch(fetchDrawingsList(project));
            expect(loadDrawingsListMock).toHaveBeenCalledTimes(1);
            expect(loadDrawingsListMock).toHaveBeenCalledWith(project.drawingsListUrl);

            // check expected store actions
            const actions = store.getActions();
            const updateAction = actions.find(a => a.type === uiFlagsActionTypes.DRAWING_LIST_UPDATED);
            expect(updateAction.drawingsList).toEqual(drawingsList);
        });
    });
});