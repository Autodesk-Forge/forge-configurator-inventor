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
    it('should return the initial state', () => {
        expect(updateParametersReducer(undefined, {})).toEqual(initialState);
    });

    const projectId = 'Conveyor';
    const parameterSet = [
        {
            name: "ABC",
            value: 123,
            changedOnUpdate: true
        },
        {
            name: "XYZ",
            value: "a string",
            other: "an attribute"
        }
    ];

    it('handles update parameters for a project with empty initial state', () => {
        const initialState = {};

        const expectedState = {};
        expectedState[projectId] = parameterSet;

        const returnedState = updateParametersReducer(initialState, updateParameters(projectId, parameterSet));
        expect(returnedState).toMatchObject(expectedState);
    });

    it('handles update parameters for a project with filled initial state', () => {
        const initialState = {};
        initialState[projectId] = parameterSet;

        const newParameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "some other string",
                other: "an attribute"
            }
        ];

        const expectedParameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "some other string",
                other: "an attribute",
                changedOnUpdate: true
            }
        ];

        const expectedState = {};
        expectedState[projectId] = expectedParameterSet;

        const returnedState = updateParametersReducer(initialState, updateParameters(projectId, newParameterSet));
        expect(returnedState).toMatchObject(expectedState);
    });

    it('handles edit a single parameter - sets value and nullifies changedOnUpdate attribute', () => {
        const initialState = {};
        initialState[projectId] = parameterSet;

        const newParameterValue = {
            name: "ABC",
            value: 234
        };

        const expectedParameterSet = [
            {
                name: "ABC",
                value: 234,
                changedOnUpdate: null
            },
            {
                name: "XYZ",
                value: "a string",
                other: "an attribute"
            }
        ];

        const expectedState = {};
        expectedState[projectId] = expectedParameterSet;

        expect(updateParametersReducer(initialState, editParameter(projectId, newParameterValue))).toStrictEqual(expectedState);
    });


    it('handles edit a single parameter - sets value and does not loose other attributes', () => {
        const initialState = {};
        initialState[projectId] = parameterSet;

        const newParameterValue = {
            name: "XYZ",
            value: "a new string"
        };

        const expectedParameterSet = [
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

        const expectedState = {};
        expectedState[projectId] = expectedParameterSet;

        expect(updateParametersReducer(initialState, editParameter(projectId, newParameterValue))).toMatchObject(expectedState);
    });

    it('does reset parameters', () => {
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