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

import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const getParameters = function(projectId, state) {
    return state[projectId];
};

export default function(state = initialState, action) {

    switch(action.type) {
        case parameterActionTypes.PARAMETERS_UPDATED: {
            const newState = { ...state };
            newState[action.projectId] = action.parameters;
            return newState;
        }

        case parameterActionTypes.PARAMETER_EDITED: {
            /* replace only the one parameter for the given project */
            const paramSet = (state[action.projectId]).map( (param) => param.name === action.parameter.name ? { ...param, value: action.parameter.value } : param);
            const newState = { ...state };
            newState[action.projectId] = paramSet;
            return newState;
        }

        case parameterActionTypes.PARAMETERS_RESET: {
            const newState = { ...state };
            newState[action.projectId] = action.parameters;
            return newState;
        }

        default:
            return state;
    }
}
