import { detectToken } from "./profileActions";
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
        ])('should remember access token (%s)',
        (hashString) => {

            window.location.hash = hashString;
            const pushStateSpy = jest.spyOn(window.history, 'pushState');

            detectToken()(store.dispatch);

            expect(repoInstance.setAccessToken).toHaveBeenCalledWith('foo');
            expect(pushStateSpy).toHaveBeenCalled();
        });

        it.each([
            "",                     // no hash
            "#",                    // hash, but nothing in it
            "#foo=1",               // different parameter
            "#access_tokennnn=1",   // slightly different name
            "#access_token=",       // expected parameter, but without value
        ])('should forget token if not found (%s)',
        (hashString) => {

            window.location.hash = hashString;

            detectToken()(store.dispatch);

            expect(repoInstance.forgetAccessToken).toHaveBeenCalled();
        });
    });

    describe('failure', () => {
        it('should log error on failure', () => {

            window.location.hash = '#access_token=foo';
            repoInstance.setAccessToken.mockImplementation(() => { throw new Error('123456'); });

            detectToken()(store.dispatch);

            expect(repoInstance.setAccessToken).toHaveBeenCalled();

            const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);
            expect(logAction).toBeDefined();
        });
    });
});
