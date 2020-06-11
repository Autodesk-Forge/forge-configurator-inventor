import repo from '../Repository';
import {addError, addLog} from './notificationActions';

const actionTypes = {
    PROFILE_LOADED: 'PROFILE_LOADED',
    LOAD_PROFILE: 'LOAD_PROFILE'
};

export default actionTypes;

export const updateProfile = profile => {
    return {
        type: actionTypes.PROFILE_LOADED,
        profile
    };
};

// eslint-disable-next-line no-unused-vars
export const loadProfile = () => async (dispatch, getState) => {
    dispatch(addLog('Load profile invoked'));
    try {
        const data = await repo.loadProfile();
        dispatch(addLog('Load profile received'));
        dispatch(updateProfile(data));
    } catch (error) {
        dispatch(addError('Failed to get profile. (' + error + ')'));
    }
};
