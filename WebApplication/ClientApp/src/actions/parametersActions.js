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
    var result = [];

    for (let key in rawParameters) {
        if (rawParameters.hasOwnProperty(key)){

            const param = rawParameters[key];

            result.push({
                name: key,
                value: param.value,
                allowedValues: param.values,
                units: param.unit,
                type: "NYI" // TODO: remove?
            });
        }
    }

    return result;
}

export const fetchParameters = (projectId) => async (dispatch, getState) => {
    dispatch(addLog('get parameters invoked'));
    try {
        const rawData = await repo.loadParameters(projectId);
        const parameters = adaptParameters(rawData);
        dispatch(addLog('parameters received'));
        dispatch(updateParameters(projectId, parameters));
    } catch (error) {
        dispatch(addError('Failed to get parameters for ' + projectId + '. (' + error + ')'));
    }
}
