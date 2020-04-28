import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import actionTypes from './projectListActions'

export const updateParameters = (projectId, parameters) => {
    return {
        type: actionTypes.PARAMETERS_UPDATED,
        projectId: projectId,
        parameters
    }
}

export const fetchParameters = (projectId) => async (dispatch, getState) => {
    dispatch(addLog('get parameters invoked'));
    try {
        const data = await repo.loadParameters(projectId);
        dispatch(addLog('parameters received'));
        dispatch(updateParameters(projectId, data.parameters));
    } catch (error) {
        dispatch(addError('Failed to get parameters for ' + projectId + '. (' + error + ')'));
    }
}
