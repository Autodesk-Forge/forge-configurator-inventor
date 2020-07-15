import actionTypes, { uploadPackage } from './uploadPackageActions';
import projectListActions from './projectListActions';
import { actionTypes as uiFlagsActionTypes } from './uiFlagsActions';

// the test based on https://redux.js.org/recipes/writing-tests#async-action-creators

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';
const uploadPackageMock = repoInstance.uploadPackage;

import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

const packageData = {};

describe('uploadPackage', () => {

    beforeEach(() => {
        uploadPackageMock.mockClear();
    });

    it('should upload Package', async () => {

        // set expected value for the mock
        uploadPackageMock.mockReturnValue(packageData);

        const store = mockStore({ uiFlags: { package: { file: {name: "a.zip"}, root: "a.asm"}} });

        await store.dispatch(uploadPackage()); // demand projects loading
        // ensure that the mock called once
        expect(uploadPackageMock).toHaveBeenCalledTimes(1);

        const actions = store.getActions();

        // check expected actions and their types
        expect(actions).toHaveLength(4); // TBD, currently just uiFlags to show and hide the progress
        expect(actions[0].type).toEqual(uiFlagsActionTypes.SHOW_UPLOAD_PACKAGE);
        expect(actions[1].type).toEqual(actionTypes.SET_UPLOAD_PROGRESS_VISIBLE);
        expect(actions[2].type).toEqual(projectListActions.ADD_PROJECT);
        expect(actions[3].type).toEqual(actionTypes.SET_UPLOAD_PROGRESS_DONE);

        // TBD check if the expected project is added to the project list
        // expect(actions[2].projectList).toEqual(testProjects);
    });

    it('should handle conflict', async () => {

        // set expected value for the mock
        uploadPackageMock.mockImplementation(() => { throw { response: { status: 409}}; });

        const store = mockStore({ uiFlags: { package: { file: {name: "a.zip"}, root: "a.asm"}} });

        await store.dispatch(uploadPackage()); // demand projects loading
        // ensure that the mock called once
        expect(uploadPackageMock).toHaveBeenCalledTimes(1);

        const actions = store.getActions();

        // check expected actions and their types
        const conflictAction = actions.find(a => a.type === uiFlagsActionTypes.PROJECT_EXISTS);
        expect(conflictAction.exists).toEqual(true);
    });


    it('should handle workitem error', async () => {

        // set expected value for the mock
        const reportUrl = 'WI report url';
        uploadPackageMock.mockImplementation(() => { throw { response: { status: 422, data: { reportUrl: reportUrl}}}; });

        const store = mockStore({ uiFlags: { package: { file: {name: "a.zip"}, root: "a.asm"}} });

        await store.dispatch(uploadPackage()); // demand projects loading
        // ensure that the mock called once
        expect(uploadPackageMock).toHaveBeenCalledTimes(1);

        const actions = store.getActions();

        // check expected actions and their types
        const uploadFailedAction = actions.find(a => a.type === actionTypes.SET_UPLOAD_FAILED);
        expect(uploadFailedAction.reportUrl).toEqual(reportUrl);
    });

    it('should do nothing when all the form values are empty', async () => {

        const store = mockStore({ uiFlags: { package: { file: null, root: ''}} });

        await store.dispatch(uploadPackage());
        expect(uploadPackageMock).toHaveBeenCalledTimes(0);

        const actions = store.getActions();

        expect(actions).toHaveLength(0);
    });

    it('should do nothing when file is empty', async () => {

        const store = mockStore({ uiFlags: { package: { file: null, root: 'asm.iam'}} });

        await store.dispatch(uploadPackage());
        expect(uploadPackageMock).toHaveBeenCalledTimes(0);

        const actions = store.getActions();

        expect(actions).toHaveLength(0);
    });

    it('should do nothing when root is empty and file is assembly', async () => {

        const store = mockStore({ uiFlags: { package: { file: {name: "a.zip"}, root: ''}} });

        await store.dispatch(uploadPackage());
        expect(uploadPackageMock).toHaveBeenCalledTimes(0);

        const actions = store.getActions();

        expect(actions).toHaveLength(0);
    });

    it('should be able to upload when root is not defined for other than zip file', async () => {

        const store = mockStore({ uiFlags: { package: { file: {name: "a.ipt"}, root: ''}} });

        uploadPackageMock.mockImplementation(() => { return { response: { status: 200 }}; });

        await store.dispatch(uploadPackage());
        expect(uploadPackageMock).toHaveBeenCalledTimes(1);

        const actions = store.getActions();

        expect(actions).toContainEqual({type: actionTypes.SET_UPLOAD_PROGRESS_VISIBLE});

    });
});
