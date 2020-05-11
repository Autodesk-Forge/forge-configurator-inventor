import repo from '../Repository';
import {addError} from './notificationActions';

export const actionTypes = {
    UPDATE_SHOW_PARAMETERS_CHANGED: 'UPDATE_SHOW_PARAMETERS_CHANGED',
    FETCH_SHOW_PARAMETERS_CHANGED: 'FETCH_SHOW_PARAMETERS_CHANGED'
};

export const updateShowParametersChanged = (show) => {
    return {
        type: actionTypes.UPDATE_SHOW_PARAMETERS_CHANGED,
        show
    };
};

export const fetchShowParametersChanged = () => async (dispatch) => {
    try {
        const showParametersChanged = await repo.loadShowParametersChanged();
        dispatch(updateShowParametersChanged(showParametersChanged));
    } catch (error) {
        dispatch(addError('Failed to get information about "show changed parameters" . (' + error + ')'));
    }
};
