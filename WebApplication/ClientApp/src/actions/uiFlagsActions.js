import repo from '../Repository';
import {addError, addLog} from './notificationActions';
import axios from 'axios';

export const actionTypes = {
    REJECT_PARAMETERS_EDITED_MESSAGE: 'REJECT_PARAMETERS_EDITED_MESSAGE',
    FETCH_SHOW_PARAMETERS_CHANGED: 'FETCH_SHOW_PARAMETERS_CHANGED',
    CLOSE_PARAMETERS_EDITED_MESSAGE: 'CLOSE_PARAMETERS_EDITED_MESSAGE',
    SHOW_UPDATE_PROGRESS: 'SHOW_UPDATE_PROGRESS',
    SHOW_RFA_PROGRESS: 'SHOW_RFA_PROGRESS',
    HIDE_RFA_PROGRESS: 'HIDE_RFA_PROGRESS',
    SET_RFA_LINK: 'SET_RFA_LINK'
};

export default actionTypes;

export const closeParametersEditedMessage = () => {
    return {
        type: actionTypes.CLOSE_PARAMETERS_EDITED_MESSAGE
    };
};

export const hideUpdateMessageBanner = (permanently) => async (dispatch) => {

    if (permanently === true) {
       const result = await repo.sendShowParametersChanged(false);
       dispatch(rejectParametersEditedMessage(!result));
    }

    dispatch(closeParametersEditedMessage());
};

export const rejectParametersEditedMessage = (show) => {
    return {
        type: actionTypes.REJECT_PARAMETERS_EDITED_MESSAGE,
        show
    };
};

export const fetchShowParametersChanged = () => async (dispatch) => {
    try {
        const showParametersChanged = await repo.loadShowParametersChanged();
        dispatch(rejectParametersEditedMessage(!showParametersChanged));
    } catch (error) {
        dispatch(addError('Failed to get information about "show changed parameters" . (' + error + ')'));
    }
};

export const showUpdateProgress = (visible) => {
    return {
        type: actionTypes.SHOW_UPDATE_PROGRESS,
        visible
    };
};

export const showRFAModalProgress = (visible) => {
    return {
        type: actionTypes.SHOW_RFA_PROGRESS,
        visible
    };
};

export const setRFALink = (url) => {
    return {
        type: actionTypes.SET_RFA_LINK,
        url
    };
};

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
            axios.defaults.headers.common['Authorization'] = accessToken;
        } else {
            dispatch(addLog('Access token is not found'));
            delete axios.defaults.headers.common['Authorization'];
        }
    } catch (error) {
        dispatch(addError('Failed to get information about "show changed parameters" . (' + error + ')'));
    }
};