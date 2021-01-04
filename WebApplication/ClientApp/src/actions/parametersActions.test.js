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

import { fetchParameters, formatParameters, updateModelWithParameters, compareParameters } from './parametersActions';
import parameterActionTypes from './parametersActions';
import notificationTypes from '../actions/notificationActions';

// following is for testing the updateModelWithParameters
// prepare mock for signalR
import { actionTypes as uiFlagsActionTypes } from './uiFlagsActions';
import { actionTypes as projectListActionTypes } from './projectListActions';
import * as signalR from '@aspnet/signalr';

const errorReportLink = 'https://error.link';
const jobId = 'job1';

import connectionMock from './connectionMock';

// end of "for testing the updateModelWithParameters"

// the test based on https://redux.js.org/recipes/writing-tests#async-action-creators

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';

import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

// prepare test data
const projectId = '1';

const fakeInventorParams = {
    p1: { value: 'unquoted', values: [] },
    p2: { value: '"unquoted"', values: [ 'foo', '"bar"' ] },
    p3: { value: 'a' },
    p4: { value: null },
};

// set expected value for the mock
const loadParametersMock = repoInstance.loadParameters;


describe('fetchParameters', () => {

    let cachedParameters;
    let store;

    beforeEach(() => {

        loadParametersMock.mockClear();

        // prepare empty 'updated parameters' data
        const fakeState = {
            updateParameters: {}
        };
        cachedParameters = fakeState.updateParameters;

        store = mockStore(fakeState);
        store.getState = () => fakeState;
    });

    describe('success', () => {

        beforeEach(() => {

            loadParametersMock.mockReturnValue(fakeInventorParams);
        });

        it('should fetch parameters from the server if there are none in the project', async () => {

            await store.dispatch(fetchParameters(projectId)); // demand parameters loading
            // ensure that the mock called once
            expect(loadParametersMock).toHaveBeenCalledTimes(1);

            const actions = store.getActions();
            const updateAction = actions.find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);

            // check expected actions and their types
            expect(updateAction.projectId).toEqual(projectId);
            expect(updateAction.parameters).toHaveLength(4); // not testing for exact content, as adaptParameters messes them up
        });

        it('should fetch parameters from the server if there is empty parameter array in the project', async () => {

            cachedParameters[projectId] = [];

            await store.dispatch(fetchParameters(projectId)); // demand parameters loading
            // ensure that the mock called once
            expect(loadParametersMock).toHaveBeenCalledTimes(1);

            const actions = store.getActions();
            const updateAction = actions.find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);

            // check expected actions and their types
            expect(updateAction.projectId).toEqual(projectId);
            expect(updateAction.parameters).toHaveLength(4); // not testing for exact content, as adaptParameters messes them up
        });

        it('should NOT fetch parameters from the server if there are SOME in the project', async () => {

            cachedParameters[projectId] = [{ name: 'JawOffset', value: '10 mm', units: 'mm' }];

            await store.dispatch(fetchParameters(projectId)); // demand parameters loading
            // ensure that the mock does not called
            expect(loadParametersMock).toHaveBeenCalledTimes(0);

            const actions = store.getActions();

            // check no update parameters is called
            expect(actions.some(a => a.type === parameterActionTypes.PARAMETERS_UPDATED)).toEqual(false);
        });

        // validate conversion from Inventor parameters to internal format
        describe('conversion', () => {

            /** Utility method to hide parameter conversion details */
            async function convertParam(inputParam) {

                loadParametersMock.mockReturnValue(inputParam);

                await store.dispatch(fetchParameters(projectId)); // demand parameters loading
                const action = store.getActions().find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);
                return action.parameters[0];
            }

            it('should load simple parameter and set defaults for optional fields', async () => {

                const simpleParam = { JawOffset: { value: '10 mm', unit: 'mm' } };
                const result = await convertParam(simpleParam);

                // check correct conversion. Note the exact object comparison, because it checks
                // optional 'readonly', 'label' and 'allowedValues' fields
                expect(result).toEqual({ name: 'JawOffset', value: '10 mm', units: 'mm', readonly: false, label: 'JawOffset', allowedValues: [] });
            });

            it('should load complex parameter', async () => {

                const choiceParam = {
                    WrenchSz: {
                        value: '"Small"',
                        unit: 'Text',
                        values: ['"Large"', '"Medium"', '"Small"']
                    }
                };

                const result = await convertParam(choiceParam);

                expect(result).toMatchObject({
                                            name: 'WrenchSz',
                                            value: 'Small', // note it's unquoted
                                            units: 'Text',
                                            allowedValues: [ 'Large', 'Medium', 'Small' ] // note it's unquoted
                                        });
            });

            it('should recognize "readonly" flag', async () => {

                const input = { JawOffset: { value: '10 mm', unit: 'mm', readonly: true } };
                const result = await convertParam(input);

                // check expected actions and their types
                expect(result).toMatchObject({ name: 'JawOffset', value: '10 mm', units: 'mm', readonly: true, label: 'JawOffset' });
            });

            it('should recognize "label" field', async () => {

                const input = { JawOffset: { value: '10 mm', unit: 'mm', label: 'Jaw Offset' } };
                const result = await convertParam(input);

                // check expected actions and their types
                expect(result).toMatchObject({ name: 'JawOffset', value: '10 mm', units: 'mm', label: 'Jaw Offset', readonly: false });
            });
        });

        // validate conversion from internal format back to Inventor parameters
        describe('conversion back', () => {

            it('correctly convert parameters', () => {

                const internalParameters = [
                    {
                        name: 'WrenchSz',
                        value: 'Small',
                        units: 'Text',
                        allowedValues: ['Large', 'Medium', 'Small'],
                        label: "Size",
                        readonly: false
                    }
                ];

                const invParameters = {
                    WrenchSz: {
                        value: '"Small"'
                    }
                };

                const formatted = formatParameters(internalParameters);
                expect(formatted).toMatchObject(invParameters);
            });
        });
    });

    describe('errors', () => {

        it('should handle server error and log it', async () => {

            loadParametersMock.mockImplementation(() => { throw new Error('123456'); });

            await store.dispatch(fetchParameters('someProjectId')); // demand parameters loading
            // find logged error
            const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);

            expect(logAction).toBeDefined();

            // log message should contains project ID and error message
            const errorMessage = logAction.info;
            expect(errorMessage).toMatch(/someProjectId/);
            expect(errorMessage).toMatch(/123456/);
        });
    });

    describe('updateModelWithParameters', () => {
        beforeAll(() => {
            // prepare mock for signalR
            signalR.HubConnectionBuilder = jest.fn();
            signalR.HubConnectionBuilder.mockImplementation(() => ({
                withUrl: function(/*url*/) {
                    return {
                        configureLogging: function(/*trace*/) {
                            return { build: function() { return connectionMock; }};
                        }
                    };
                }
            }));
        });

        beforeEach(() => { // Runs before each test in the suite
            store.clearActions();
        });

        it('check updateModelWithParameters success path', async () => {
            await store.dispatch(updateModelWithParameters(projectId, []));
            const parameters = { "a1": { "value": "7", "values": [], "unit": "mm" } };
            const adaptedParams = [ { "name": "a1", "value": "7", "allowedValues": [], "units": "mm", "readonly": false, "label": "a1" } ];
            const projectData = { "data": "someData" };
            const updatedState = {
                parameters: parameters,
                ...projectData
            };
            const theStats = { credits: 1 };

            connectionMock.simulateComplete(updatedState, theStats);

            // check expected store actions
            const actions = store.getActions();
            // there are two SET_REPORT_URL actions in the list. The first one come from job start and is called with null to clear old data...
            const updateParams = actions.find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);
            expect(updateParams.projectId).toEqual(projectId);
            expect(updateParams.parameters).toEqual(adaptedParams);
            const updateProject = actions.find(a => a.type === projectListActionTypes.UPDATE_PROJECT);
            expect(updateProject.activeProjectId).toEqual(projectId);
            expect(updateProject.data).toEqual(projectData);
            // verify stats
            expect(actions.some(a => a.type === uiFlagsActionTypes.SET_STATS && a.stats === theStats)).toBeTruthy();
        });

        it('check updateModelWithParameters error path', async () => {
            await store.dispatch(updateModelWithParameters(projectId, []));
            connectionMock.simulateErrorWithReport(jobId, errorReportLink);

            // check expected store actions
            const actions = store.getActions();
            // there are two SET_REPORT_URL actions in the list. The first one come from job start and is called with null to clear old data...
            expect(actions.some(a => (a.type === uiFlagsActionTypes.SET_ERROR_DATA && a.errorData?.reportUrl === errorReportLink))).toEqual(true);
            expect(actions.some(a => a.type === uiFlagsActionTypes.SHOW_UPDATE_FAILED)).toEqual(true);
        });
    });

    describe('parameter comparison', () => {
        const first = {
            value: "12000 mm",
            units: "mm"
        };

        it('makes exact match', () => {
            expect(compareParameters(first, first)).toBeTruthy();
        });

        it('makes match if units ommited', () => {
            expect(compareParameters(first, { value: "12000", units: "mm"})).toBeTruthy();
        });

        it('does not make match if units differ in both value and units', () => {
            expect(compareParameters(first, { value: "12000 in", units: "in"})).toBeFalsy();
        });

        it('does not make match if units differ in value only', () => {
            expect(compareParameters(first, { value: "12000 in", units: "mm"})).toBeFalsy();
        });

        it('does not make match if units differ and not present in value', () => {
            expect(compareParameters(first, { value: "12000", units: "in"})).toBeFalsy();
        });

        it('does not make match if units same in units attribute, but are different than what is in both values', () => {
            expect(compareParameters({ value: "12000 cm", units: "mm"}, { value: "12000 km", units: "mm"})).toBeFalsy();
        });

        it('handles undefined parameters', () => {
            expect(compareParameters(first, undefined)).toBeFalsy();
            expect(compareParameters(first, null)).toBeFalsy();
        });
    });
});
