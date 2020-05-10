import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import { getProject } from '../reducers/mainReducer';

const actionTypes = {
    PARAMETERS_UPDATED: 'PARAMETERS_UPDATED',
    PARAMETER_EDITED: 'PARAMETER_EDITED',
    PARAMETERS_RESET: 'PARAMETERS_RESET'
};

export default actionTypes;

export const updateParameters = (projectId, parameters) => {
    return {
        type: actionTypes.PARAMETERS_UPDATED,
        projectId,
        parameters
    };
};

export const editParameter = (projectId, parameter) => {
    return {
        type: actionTypes.PARAMETER_EDITED,
        projectId,
        parameter
    };
};

export const resetParameters = (projectId) => {
    return {
        type: actionTypes.PARAMETERS_RESET,
        projectId
    };
};

/**
 * Convert incoming raw parameters into expected parameters format.
 * @param {Object} rawParameters Object with parameter data.
 * The raw data looks like:
 * {
 *       "WrenchSz": {
 *           "values": [
 *           "\"Large\"",
 *           "\"Medium\"",
 *           "\"Small\""
 *           ],
 *           "value": "\"Small\"",
 *           "unit": "Text"
 *       },
 *       "JawOffset": {
 *           "values": [],
 *           "value": "10 mm",
 *           "unit": "mm"
 *       }
 * }
 */
function adaptParameters(rawParameters) {
    return Object.entries(rawParameters).map( ([key, param]) => {
        return {
            name: key,
            value: param.value,
            allowedValues: param.values,
            units: param.unit,
            type: "NYI" // TODO: remove?
        };
    });
}

// eslint-disable-next-line no-unused-vars
export const fetchParameters = (projectId) => async (dispatch, getState) => {
    const selectedProject = getProject(projectId, getState());
    if(selectedProject && selectedProject.updateParameters && selectedProject.updateParameters.length!==0) {
        return;
    }

    dispatch(addLog('get parameters invoked'));
    try {
        const rawData = await repo.loadParameters(projectId);
        const parameters = adaptParameters(rawData);
        dispatch(addLog('parameters received'));
        dispatch(updateParameters(projectId, parameters));
    } catch (error) {
        dispatch(addError('Failed to get parameters for ' + projectId + '. (' + error + ')'));
    }
};
