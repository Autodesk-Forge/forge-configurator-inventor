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

import {notificationReducer, initialState} from './notificationReducer';
import {addError, addLog} from '../actions/notificationActions';

describe('notification reducer', () => {
    test('should return the initial state', () => {
        expect(notificationReducer(undefined, {})).toEqual(initialState);
    });

    test('handles adding an error', () => {
        const newText = 'Some Error';
        const stateWithError = [
            '0 Errors',
            newText
        ];
        expect(notificationReducer(initialState, addError(newText))).toEqual(stateWithError);
    });

    test('handles adding a log', () => {
        const newText = 'A Log';
        const stateWithLog = [
            '0 Errors',
            newText
        ];
        expect(notificationReducer(initialState, addLog(newText))).toEqual(stateWithLog);
    });
});