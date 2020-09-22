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
import {updateDrawingsList} from './uiFlagsActions';

export const fetchDrawingsList = (project) => async (dispatch) => {
    if(!project.id) return;

    dispatch(addLog('Load Drawings list invoked'));
    try {
        const data = await repo.loadDrawingsList(project.drawingsListUrl);
        dispatch(addLog('Drawings list received'));
        dispatch(updateDrawingsList(data));
    } catch (error) {
        dispatch(addError('Failed to get Drawings list for ' + project.id + '. (' + error + ')'));
    }
};
