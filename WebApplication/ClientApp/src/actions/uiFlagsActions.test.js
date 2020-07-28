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

import Enzyme from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import * as uiFlagsActions from './uiFlagsActions';

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';

Enzyme.configure({ adapter: new Adapter() });

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);
const mockState = {
  uiFlags: {
  }
};
const store = mockStore(mockState);

describe('uiFlagsActions', () => {
    beforeEach(() => { // Runs before each test in the suite
        store.clearActions();
    });

    it('check closeParametersEditedMessage action', () => {
        store.dispatch(uiFlagsActions.closeParametersEditedMessage());
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check hideUpdateMessageBanner action', async () => {
        const sendShowParametersChangedMock = repoInstance.sendShowParametersChanged;
        sendShowParametersChangedMock.mockReturnValue(true);

        await store.dispatch(uiFlagsActions.hideUpdateMessageBanner(true));
        expect(sendShowParametersChangedMock).toHaveBeenCalledTimes(1);
        expect(store.getActions()).toMatchSnapshot();
    });

    it ('check fetchShowParametersChanged action', async () => {
        const loadShowParametersChangedMock = repoInstance.loadShowParametersChanged;
        await store.dispatch(uiFlagsActions.fetchShowParametersChanged(true));
        expect(loadShowParametersChangedMock).toHaveBeenCalledTimes(1);
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check rejectParametersEditedMessage action', () => {
        store.dispatch(uiFlagsActions.rejectParametersEditedMessage(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check showModalProgress action', () => {
        store.dispatch(uiFlagsActions.showModalProgress(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check showUpdateFailed action', () => {
        store.dispatch(uiFlagsActions.showUpdateFailed(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check showRFAModalProgress action', () => {
        store.dispatch(uiFlagsActions.showRFAModalProgress(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check setRFALink action', () => {
        store.dispatch(uiFlagsActions.setRFALink("link"));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check showUploadPackage action', () => {
        store.dispatch(uiFlagsActions.showUploadPackage(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check editPackageFile action', () => {
        store.dispatch(uiFlagsActions.editPackageFile("file"));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check editPackageRoot action', () => {
        store.dispatch(uiFlagsActions.editPackageRoot("root"));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check updateActiveTabIndex action', () => {
        store.dispatch(uiFlagsActions.updateActiveTabIndex(1));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check setProjectAlreadyExists action', () => {
        store.dispatch(uiFlagsActions.setProjectAlreadyExists(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check showDeleteProject action', () => {
        store.dispatch(uiFlagsActions.showDeleteProject(true));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check setCheckedProjects action', () => {
        store.dispatch(uiFlagsActions.setCheckedProjects(['1','2']));
        expect(store.getActions()).toMatchSnapshot();
    });

    it('check clearCheckedProjects action', () => {
        store.dispatch(uiFlagsActions.clearCheckedProjects());
        expect(store.getActions()).toMatchSnapshot();
    });
});
