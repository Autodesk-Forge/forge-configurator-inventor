import actionTypes, { detectToken, loadProfile } from "./profileActions";
import notificationTypes from '../actions/notificationActions';

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';

import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('detectToken', () => {

    let store;
    beforeEach(() => {
        store = mockStore({});
        repoInstance.setAccessToken.mockClear();
        repoInstance.forgetAccessToken.mockClear();
    });

    describe('success', () => {

        it.each([
            "#access_token=foo",
            "#first=second&access_token=foo",
        ])("should remember access token if it's in the url (%s)",
        (hashString) => {

            window.location.hash = hashString;
            const pushStateSpy = jest.spyOn(window.history, 'pushState');

            detectToken()(store.dispatch);

            expect(repoInstance.setAccessToken).toHaveBeenCalledWith('foo');
            expect(pushStateSpy).toHaveBeenCalled();

            pushStateSpy.mockRestore();
        });

        it.each([
            "",                     // no hash
            "#",                    // hash, but nothing in it
            "#foo=1",               // different parameter
            "#access_tokennnn=1",   // slightly different name
            "#access_token=",       // expected parameter, but without value
        ])('should forget token if not found in url (%s)',
        (hashString) => {

            window.location.hash = hashString;

            detectToken()(store.dispatch);

            expect(repoInstance.forgetAccessToken).toHaveBeenCalled();
        });
    });

    describe('failure', () => {
        it('should log error on failure and forget access token', () => {

            // prepare to raise error during token extraction
            window.location.hash = '#access_token=foo';
            repoInstance.setAccessToken.mockImplementation(() => { throw new Error('123456'); });

            // execute
            detectToken()(store.dispatch);

            // check if error is logged and token is forgotten
            expect(repoInstance.setAccessToken).toHaveBeenCalled();

            const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);
            expect(logAction).toBeDefined();

            expect(repoInstance.forgetAccessToken).toHaveBeenCalled();
        });
    });
});

describe('loadProfile', () => {

    let store;
    beforeEach(() => {
        store = mockStore({});
        repoInstance.loadProfile.mockClear();
    });

    describe('success', () => {

        it('should fetch profile from repository', async () => {

            const profile = { name: "John Smith", avatarUrl: "http://johnsmith.com/avatar.jpg"};

            repoInstance.loadProfile.mockImplementation(() => profile);

            await store.dispatch(loadProfile());
            expect(repoInstance.loadProfile).toHaveBeenCalledTimes(1);

            // check the loaded profile is in store now
            const profileLoadedAction = store.getActions().find(a => a.type === actionTypes.PROFILE_LOADED);
            expect(profileLoadedAction.profile).toEqual(profile);
        });
    });

    describe('failure', () => {
        it('should log error on failure and forget access token', async () => {

            repoInstance.loadProfile.mockImplementation(() => { throw new Error(); });

            await store.dispatch(loadProfile());
            expect(repoInstance.loadProfile).toHaveBeenCalledTimes(1);

            // check the error is logged
            const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);
            expect(logAction).toBeDefined();
        });
    });
});
