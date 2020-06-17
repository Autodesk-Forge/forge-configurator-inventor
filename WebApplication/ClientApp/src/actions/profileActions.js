import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import axios from 'axios';

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
            dispatch(addLog(`Detected access token = '${accessToken}'`));
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
        const isLoggedIn = axios.defaults.headers.common['Authorization'];
        dispatch(updateProfile(profile, isLoggedIn));
    } catch (error) {
        dispatch(addError('Failed to get profile. (' + error + ')'));
    }
};
