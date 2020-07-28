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

import repo from '../Repository';
import {addError, addLog} from './notificationActions';

export const actionTypes = {
    PROJECT_LIST_UPDATED: 'PROJECT_LIST_UPDATED',
    ACTIVE_PROJECT_UPDATED: 'ACTIVE_PROJECT_UPDATED',
    UPDATE_PROJECT: 'UPDATE_PROJECT',
    ADD_PROJECT: 'ADD_PROJECT'
};

export default actionTypes;

export const updateProjectList = projectList => {
    return {
        type: actionTypes.PROJECT_LIST_UPDATED,
        projectList
    };
};

export const updateActiveProject = activeProjectId => {
    return {
        type: actionTypes.ACTIVE_PROJECT_UPDATED,
        activeProjectId
    };
};

export const updateProject = (activeProjectId, data) => {
    return {
        type: actionTypes.UPDATE_PROJECT,
        activeProjectId, data
    };
};

export const addProject = (newProject) => {
    return {
        type: actionTypes.ADD_PROJECT,
        newProject
    };
};

// eslint-disable-next-line no-unused-vars
export const fetchProjects = () => async (dispatch, getState) => {
    dispatch(addLog('Load Projects invoked'));
    try {
        const data = await repo.loadProjects();
        dispatch(addLog('Load Projects received'));
        dispatch(updateProjectList(data));
    } catch (error) {
        dispatch(addError('Failed to get Project list. (' + error + ')'));
    }
};
