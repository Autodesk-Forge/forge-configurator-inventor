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

import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';

import {Provider} from 'react-redux';
import {createStore, applyMiddleware} from 'redux';
import thunk from 'redux-thunk';

import {mainReducer} from './reducers/mainReducer';

import "@hig/fonts/build/ArtifaktElement.css";

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

// try to check external 'Script error.' to avoid hangs when running
// PR checks/deploying build
if (window != null) {
    window.addEventListener('error', function(event) {
        if (event.filename?.length > 0) {
            // it is some internal error, lets handle it default way
            // eslint-disable-next-line no-console
            console.log('internal error');
            return false;
        } else if (event.message === "Script error.") {
            // eslint-disable-next-line no-console
            console.log('external script error');
            return true;
        }

        return false;
    });
} else {
    // eslint-disable-next-line no-console
    console.log('no window -> no external script error handling');
}

ReactDOM.render(
<Provider store={store}>
  <App />
</Provider>, rootElement
);
