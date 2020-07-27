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

import updateParametersReducer, { initialState } from './updateParametersReducer';
import { updateParameters, editParameter, resetParameters } from '../actions/parametersActions';

describe('updateParameters reducer', () => {
    test('should return the initial state', () => {
        expect(updateParametersReducer(undefined, {})).toEqual(initialState);
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

        const returnedState = updateParametersReducer(initialState, updateParameters(projectId, parameterSet));
        expect(returnedState).toMatchObject(expectedState);
    });

    it('handles edit a single parameter - sets value and does not loose other attributes', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string",
                other: "an attribute"
            }
        ];

        const initialState = {};
        initialState[projectId] = parameterSet;

        const newParameterValue = {
            name: "XYZ",
            value: "a new string"
        };

        const newParameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a new string",
                other: "an attribute"
            }
        ];

        const newState = {};
        newState[projectId] = newParameterSet;

        expect(updateParametersReducer(initialState, editParameter(projectId, newParameterValue))).toMatchObject(newState);
    });

    it('does reset parameters', () => {
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

        const newParameterSet = [
            {
                name: "DEF",
                value: 456
            },
            {
                name: "XYZ",
                value: "a string reset"
            }
        ];

        const expectedState = {};
        expectedState[projectId] = newParameterSet;

        expect(updateParametersReducer(initialState, resetParameters(projectId, newParameterSet))).toMatchObject(expectedState);
    });
});