import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';

import {Provider} from 'react-redux';
import {createStore, applyMiddleware} from 'redux';
import thunk from 'redux-thunk';

import {mainReducer} from './reducers/mainReducer';

const createStoreWithMiddleware = applyMiddleware(thunk)(createStore);
const store = createStoreWithMiddleware(mainReducer);

// based on https://github.com/reduxjs/redux/issues/303#issuecomment-125184409
function observeStore(store, select, onChange) {
    let currentState;

    function handleChange() {
        const nextState = select(store.getState());
        if (nextState !== currentState) {
            onChange(currentState, nextState);
            currentState = nextState;
        }
    }

    const unsubscribe = store.subscribe(handleChange);
    handleChange();
    return unsubscribe;
}

const logSelector = (state) => state.notifications;

/** Dump log messages into browser console */
function logToConsole(oldMessages, newMessages) { // TODO: not clear how to differentiate 'information' from 'error' messages

  const from = oldMessages ? oldMessages.length : 0;
    for (let i = from; i < newMessages.length; i++) {
        // eslint-disable-next-line no-console
        console.log(newMessages[i]);
    }
}

// listen for notifications changes and stream them into console
observeStore(store, logSelector, logToConsole);

// const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
<Provider store={store}>
  <App />
</Provider>, rootElement
);
