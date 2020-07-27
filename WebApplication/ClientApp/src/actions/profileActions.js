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

import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import { showLoginFailed } from './uiFlagsActions';

const actionTypes = {
    PROFILE_LOADED: 'PROFILE_LOADED'
};

export default actionTypes;

/** Extract access token from URL hash */
function extractToken(urlHash) {
    const regex = /access_token=([^&]*)/g;
    const m = regex.exec(urlHash);
    return m ? m[1] : undefined;
}

export const detectToken = () => (dispatch) => {
    try {

        const accessToken = extractToken(window.location.hash.substring(1));
        if (accessToken) {
            dispatch(addLog(`Detected access token`));
            repo.setAccessToken(accessToken);

            // remove token from URL
            window.history.pushState("", document.title, window.location.pathname);
        } else {
            dispatch(addLog('Access token is not found'));
            repo.forgetAccessToken();
        }
    } catch (error) {
        dispatch(addError('Failed to detect token. (' + error + ')'));
        repo.forgetAccessToken();
    }
};

export const updateProfile = (profile, isLoggedIn) => {
    return {
        type: actionTypes.PROFILE_LOADED,
        profile,
        isLoggedIn
    };
};

export const loadProfile = () => async (dispatch) => {
    dispatch(addLog('Load profile invoked'));
    try {
        const profile = await repo.loadProfile();
        dispatch(addLog('Load profile received'));
        const isLoggedIn = repo.hasAccessToken();
        dispatch(updateProfile(profile, isLoggedIn));
    } catch (error) {
        if (error.response && error.response.status === 403) {
            dispatch(showLoginFailed(true));
        } else {
            dispatch(addError('Failed to get profile. (' + error + ')'));
        }
    }
};
