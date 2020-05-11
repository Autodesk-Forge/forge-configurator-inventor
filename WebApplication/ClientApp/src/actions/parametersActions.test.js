import { fetchParameters } from './parametersActions';
import parameterActionTypes from './parametersActions';
import notificationTypes from '../actions/notificationActions';

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

        it('should fetch parameters from the server if there are none in the project', () => {

            return store
                .dispatch(fetchParameters(projectId)) // demand parameters loading
                .then(() => {

                    // ensure that the mock called once
                    expect(loadParametersMock).toHaveBeenCalledTimes(1);

                    const actions = store.getActions();
                    const updateAction = actions.find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);

                    // check expected actions and their types
                    expect(updateAction.projectId).toEqual(projectId);
                    expect(updateAction.parameters).toHaveLength(4); // not testing for exact content, as adaptParameters messes them up
                });
        });

        it('should fetch parameters from the server if there is empty parameter array in the project', () => {

            cachedParameters[projectId] = [];

            return store
                .dispatch(fetchParameters(projectId)) // demand parameters loading
                .then(() => {

                    // ensure that the mock called once
                    expect(loadParametersMock).toHaveBeenCalledTimes(1);

                    const actions = store.getActions();
                    const updateAction = actions.find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);

                    // check expected actions and their types
                    expect(updateAction.projectId).toEqual(projectId);
                    expect(updateAction.parameters).toHaveLength(4); // not testing for exact content, as adaptParameters messes them up
            });
        });

        it('should NOT fetch parameters from the server if there are SOME in the project', () => {

            cachedParameters[projectId] = [{ name: 'JawOffset', value: '10 mm', units: 'mm' }];

            return store
                .dispatch(fetchParameters(projectId)) // demand parameters loading
                .then(() => {

                    // ensure that the mock does not called
                    expect(loadParametersMock).toHaveBeenCalledTimes(0);

                    const actions = store.getActions();

                    // check no update parameters is called
                    expect(actions.some(a => a.type === parameterActionTypes.PARAMETERS_UPDATED)).toEqual(false);
            });
        });

        // validate conversion from Inventor parameters to internal format
        describe('conversion', () => {

            it('should load simple parameter', () => {

                const simpleParam = { JawOffset: { value: '10 mm', unit: 'mm' } };
                loadParametersMock.mockReturnValue(simpleParam);

                return store
                    .dispatch(fetchParameters(projectId)) // demand parameters loading
                    .then(() => {

                        const action = store.getActions().find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);

                        // check expected actions and their types
                        expect(action.parameters[0]).toMatchObject({ name: 'JawOffset', value: '10 mm', units: 'mm' });
                });
            });

            it('should load complex parameter', () => {

                const choiceParam = {
                    WrenchSz: {
                        value: '"Small"',
                        unit: 'Text',
                        values: ['"Large"', '"Medium"', '"Small"']
                    }
                };
                loadParametersMock.mockReturnValue(choiceParam);

                return store
                    .dispatch(fetchParameters(projectId)) // demand parameters loading
                    .then(() => {

                        const action = store.getActions().find(a => a.type === parameterActionTypes.PARAMETERS_UPDATED);

                        const result = {
                            name: 'WrenchSz',
                            value: 'Small', // note it's unquoted // ER: potential problem with backward conversion
                            units: 'Text',
                            allowedValues: [ 'Large', 'Medium', 'Small' ] // note it's unquoted // ER: potential problem with backward conversion
                        };
                        expect(action.parameters[0]).toMatchObject(result);
                });
            });


        });
    });

    describe('errors', () => {

        it('should handle server error and log it', () => {

            loadParametersMock.mockImplementation(() => { throw new Error('123456'); });

            return store
                .dispatch(fetchParameters('someProjectId')) // demand parameters loading
                .then(() => {

                    // find logged error
                    const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);

                    expect(logAction).toBeDefined();

                    // log message should contains project ID and error message
                    const errorMessage = logAction.info;
                    expect(errorMessage).toMatch(/someProjectId/);
                    expect(errorMessage).toMatch(/123456/);
                });
        });
        });
});
