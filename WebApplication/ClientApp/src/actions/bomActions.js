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
import { addError, addLog } from './notificationActions';

const actionTypes = {
    BOM_UPDATED: 'BOM_UPDATED'
};

export default actionTypes;

export const updateBom = (projectId, bomData) => {
    return {
        type: actionTypes.BOM_UPDATED,
        projectId,
        bomData
    };
};

export const fetchBom = (project) => async (dispatch) => {
    if (! project.id) return;

    dispatch(addLog('get bom invoked'));
    try {
        const bomData = await repo.loadBom(project.bomJsonUrl);
        dispatch(addLog('bom received'));
        dispatch(updateBom(project.id, bomData));
    } catch (error) {
        dispatch(addError('Failed to get bom for ' + project.id + '. (' + error + ')'));
    }
};
