import Enzyme from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import * as downloadActions from './downloadActions';
import { actionTypes as uiFlagsActionTypes } from './uiFlagsActions';

const rfaLink = 'https://some.link';
const tokenMock = 'theToken';
const fullLink = `${rfaLink}/${tokenMock}`;

// prepare mock for signalR
var connectionMock = {
    onHandlers: {},
    start: function() {},
    on: function(name, fn) { 
        this.onHandlers[name] = fn; 
    },
    invoke: function() {},
    stop: function() {},
    simulateComplete: function(link) { 
        this.onHandlers['onComplete'](link); 
    },
    simulateError: function() { 
        this.onHandlers['onError'](); 
    }
};

import * as signalR from '@aspnet/signalr';
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

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';
repoInstance.getAccessToken.mockImplementation(() => tokenMock);

Enzyme.configure({ adapter: new Adapter() });

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

const mockState = {
  uiFlags: {
  }
};
const store = mockStore(mockState);

describe('downloadActions', () => {
    beforeEach(() => { // Runs before each test in the suite
        store.clearActions();
    });

    it('check getRFADownloadLink action', () => {
        return store.dispatch(downloadActions.getRFADownloadLink("ProjectId", "temp_url"))
        .then(() => {
            // simulate conection.onComplete(rfaLink);
            connectionMock.simulateComplete(rfaLink);

            // check expected store actions
            const actions = store.getActions();
            const linkAction = actions.find(a => a.type === uiFlagsActionTypes.SET_RFA_LINK);
            expect(linkAction.url).toEqual(fullLink);
        });
    });
});