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

import { deleteProject } from './deleteProjectActions';
import { actionTypes as projectListActionTypes } from './projectListActions';
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
const deleteProjectsMock = repoInstance.deleteProjects;
const loadProjectsMock = repoInstance.loadProjects;

const projectsToDelete = [ '1', '3' ];
const remainingProjects = [ { id: '2'} ];

describe('deleteProject', () => {

    let store;

    beforeEach(() => {

        deleteProjectsMock.mockClear();
        loadProjectsMock.mockClear();
        loadProjectsMock.mockResolvedValue(remainingProjects);

        // prepare empty 'updated parameters' data
        const state = {
            projectList: {
                activeProjectId: '1',
                projects: [ { id: '1'}, { id: '2'}, { id: '3'}  ]
            },
            uiFlags: {
                modalProgressShowing: false,
                showDeleteProject: false,
                checkedProjects: projectsToDelete
             },
        };

        store = mockStore(state);
        store.getState = () => state;
    });

    describe('success', () => {
        it('refreshes projects and checkboxes after delete', async () => {

            await store.dispatch(deleteProject());
            expect(deleteProjectsMock).toHaveBeenCalledTimes(1);
            expect(deleteProjectsMock).toBeCalledWith(projectsToDelete);
            expect(loadProjectsMock).toHaveBeenCalledTimes(1);

            // check expected store actions
            const actions = store.getActions();
            const updateAction = actions.find(a => a.type === projectListActionTypes.PROJECT_LIST_UPDATED);
            expect(updateAction.projectList).toEqual(remainingProjects);

            expect(actions.some(a => a.type === uiFlagsActionTypes.CLEAR_CHECKED_PROJECTS)).toEqual(true);
        });
    });
});