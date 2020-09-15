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

import drawingListsActionTypes from '../actions/drawingsListActions';

export const initialState = {
    activeDrawing: null
};

export const getDrawingsList = function(state) {
    return state.drawings;
};

export const getActiveDrawing = function(state) {
    return state.activeDrawing;
};

/** Generate shallow array clone, sorted by drawing name. */
export function sortDrawings(drawings) {
    return [].concat(drawings).sort((a,b) => {

        const shortA = a.split('\\').pop().split('/').pop();
        const shortB = b.split('\\').pop().split('/').pop();

        return shortA.localeCompare(shortB);
    });
}

export default function(state = initialState, action) {
    switch(action.type) {
        case drawingListsActionTypes.DRAWING_LIST_UPDATED: {

            const sortedDrawingsList = sortDrawings(action.drawingsList);
            const firstDrawing = "BWheelAssembly2.idw";
            return { activeDrawing: firstDrawing, drawings: sortedDrawingsList };
        }
        case drawingListsActionTypes.ACTIVE_DRAWING_UPDATED: {
            return { ...state, activeDrawing: action.activeDrawing};
        }
        default:
            return state;
    }
}