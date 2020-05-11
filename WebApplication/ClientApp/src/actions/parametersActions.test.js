import { fetchParameters } from './parametersActions';
import parameterActionTypes from './parametersActions';

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

const testParameters = [
    { name: 'parameter one', value: 'unquoted', values: [] },
    { name: 'parameter two', value: '"unquoted"', values: [ 'foo', '"bar"' ] },
    { name: 'parameter three', value: 'a' },
    { name: 'parameter four', value: null },
];

// set expected value for the mock
const loadParametersMock = repoInstance.loadParameters;


describe('fetchParameters', () => {

    let updateParameters;
    let store;
    beforeEach(() => {

        loadParametersMock.mockReturnValue(testParameters);
        loadParametersMock.mockClear();

        // prepare empty 'updated parameters' data
        const fakeState = {
            updateParameters: {}
        };
        updateParameters = fakeState.updateParameters;


        store = mockStore(fakeState);
        store.getState = () => fakeState;
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

        updateParameters[projectId] = [];

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

        updateParameters[projectId] = testParameters;

        return store
            .dispatch(fetchParameters(projectId, store.getState)) // demand parameters loading
            .then(() => {

                // ensure that the mock does not called
                expect(loadParametersMock).toHaveBeenCalledTimes(0);

                const actions = store.getActions();

                // check no update parameters is called
                expect(actions.some(a => a.type === parameterActionTypes.PARAMETERS_UPDATED)).toEqual(false);
        });
    });
});
