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

import parametersReducer, { initialState } from './parametersReducer';
import { updateParameters, editParameter, resetParameters } from '../actions/parametersActions';

describe('parameters reducer', () => {
    test('should return the initial state', () => {
        expect(parametersReducer(undefined, {})).toEqual(initialState);
    });

    test('handles update parameters for a project', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string"
            }
        ];

        const initialState = {};

        const expectedState = {};
        expectedState[projectId] = parameterSet;

        expect(parametersReducer(initialState, updateParameters(projectId, parameterSet))).toMatchObject(expectedState);
    });

    it('does nothing on edit', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string"
            }
        ];

        const initialState = {};
        initialState[projectId] = parameterSet;

        const newParameterValue = {
            name: "XYZ",
            value: "a new string"
        };

        expect(parametersReducer(initialState, editParameter(projectId, newParameterValue))).toMatchObject(initialState);
    });

    it('does nothing on reset', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string"
            }
        ];

        const initialState = {};
        initialState[projectId] = parameterSet;

        expect(parametersReducer(initialState, resetParameters(projectId))).toMatchObject(initialState);
    });
});