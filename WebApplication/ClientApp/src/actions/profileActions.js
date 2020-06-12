import repo from '../Repository';
import {addError, addLog} from './notificationActions';

const actionTypes = {
    PROFILE_LOADED: 'PROFILE_LOADED',
    LOAD_PROFILE: 'LOAD_PROFILE'
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
        dispatch(addError('Failed to get information about "show changed parameters" . (' + error + ')'));
    }
};

export const updateProfile = profile => {
    return {
        type: actionTypes.PROFILE_LOADED,
        profile
    };
};

export const loadProfile = () => async (dispatch) => {
    dispatch(addLog('Load profile invoked'));
    try {
        const data = await repo.loadProfile();
        dispatch(addLog('Load profile received'));
        dispatch(updateProfile(data));
    } catch (error) {
        dispatch(addError('Failed to get profile. (' + error + ')'));
    }
};
