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
    ACTIVE_DRAWING_UPDATED: 'ACTIVE_DRAWING_UPDATED',
    DRAWING_LIST_UPDATED: 'DRAWING_LIST_UPDATED'
};

export default actionTypes;

export const updateActiveDrawing = activeDrawing => {
    return {
        type: actionTypes.ACTIVE_DRAWING_UPDATED,
        activeDrawing
    };
};

export const updateDrawingsList = drawingsList => {
    return {
        type: actionTypes.DRAWING_LIST_UPDATED,
        drawingsList
    };
};

// eslint-disable-next-line no-unused-vars
export const fetchDrawingsList = (project) => async (dispatch) => {
    if(!project.id) return;

    dispatch(addLog('Load Drawings list invoked'));
    try {
        const data = await repo.loadDrawingsList(project.drawingsListUrl);
        dispatch(addLog('Drawings list received'));
        dispatch(updateDrawingsList(project.id, data));
    } catch (error) {
        dispatch(addError('Failed to get Drawings list for ' + project.id + '. (' + error + ')'));
    }
};
