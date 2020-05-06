import { fetchParameters } from './parametersActions';

// the test based on https://redux.js.org/recipes/writing-tests#async-action-creators

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';
const loadParametersMock = repoInstance.loadParameters;

import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

const testProjectsNoParams = {
    projectList: [
        {
            id: '1',
            label: 'Local Project 1',
            image: 'bike.png'
        }
    ]
};

const testProjectsEmptyParams = {
    projectList: [
        {
            id: '1',
            label: 'Local Project 1',
            image: 'bike.png',
            parameters: [],
            updateParameters: []
        }
    ]
};

const testProjectsHasParams = {
    projectList: [
        {
            id: '1',
            label: 'Local Project 1',
            image: 'bike.png',
            parameters: [ { name: 'a parameter here' } ],
            updateParameters: [ { name: 'a parameteter here' } ]        
        }
    ]
}

const testParameters = [
    { name: 'parameter one' },
    { name: 'parameter two' }
];

describe('fetchParameters', () => {

    beforeEach(() => {
        loadParametersMock.mockClear();
    });

    it('should fetch parameters from the server if there are none in the project', () => {

        // set expected value for the mock
        loadParametersMock.mockReturnValue(testParameters);

        const store = mockStore(testProjectsNoParams);

        return store
            .dispatch(fetchParameters('1')) // demand parameters loading
            .then(() => {

                // ensure that the mock called once
                expect(loadParametersMock).toHaveBeenCalledTimes(1);

                const actions = store.getActions();
                const updateAction = actions.find(a => a.type === 'PARAMETERS_UPDATED');

                // check expected actions and their types
                expect(updateAction.projectId).toEqual('1');
                expect(updateAction.parameters.length).toEqual(2); // not testing for exact content, as adaptParameters messes them up
        });
    });

    it('should fetch parameters from the server if there is empty parameter array in the project', () => {

        // set expected value for the mock
        loadParametersMock.mockReturnValue(testParameters);

        const store = mockStore(testProjectsEmptyParams);

        return store
            .dispatch(fetchParameters('1')) // demand parameters loading
            .then(() => {

                // ensure that the mock called once
                expect(loadParametersMock).toHaveBeenCalledTimes(1);

                const actions = store.getActions();
                const updateAction = actions.find(a => a.type === 'PARAMETERS_UPDATED');

                // check expected actions and their types
                expect(updateAction.projectId).toEqual('1');
                expect(updateAction.parameters.length).toEqual(2); // not testing for exact content, as adaptParameters messes them up
        });
    });

    it('should NOT fetch parameters from the server if there are SOME in the project', () => {

        // set expected value for the mock
        loadParametersMock.mockReturnValue(testParameters);

        const store = mockStore(testProjectsHasParams);

        return store
            .dispatch(fetchParameters('1')) // demand parameters loading
            .then(() => {

                // ensure that the mock called once
                expect(loadParametersMock).toHaveBeenCalledTimes(0);

                const actions = store.getActions();

                // check no update parameters is called
                expect(actions.some(a => a.type === 'PARAMETERS_UPDATED')).toEqual(false);
        });
    });        

});
