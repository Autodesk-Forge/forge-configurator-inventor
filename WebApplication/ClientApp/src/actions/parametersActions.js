import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import actionTypes from './projectListActions';

export const updateParameters = (projectId, parameters) => {
    return {
        type: actionTypes.PARAMETERS_UPDATED,
        projectId: projectId,
        parameters
    };
};

export const editParameter = (projectId, parameter) => {
    return {
        type: actionTypes.PARAMETER_EDITED,
        projectId: projectId,
        parameter
    };
};

export const resetParameters = (projectId) => {
    return {
        type: actionTypes.PARAMETERS_RESET,
        projectId: projectId
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
    var unquote = function(input) {
        if (input == null || input.length < 2)
            return input;
        
        if (input[0] === "\"" && input[input.length-1] === "\"")
            return input.substr(1, input.length-2);

        return input.replace('"\"','"');
    };

    return Object.entries(rawParameters).map( ([key, param]) => {
        return {
            name: key,
            value: unquote(param.value),
            allowedValues: param.values?.map( item => unquote(item) ),
            units: param.unit,
            type: "NYI" // TODO: remove?
        };
    });
};

// eslint-disable-next-line no-unused-vars
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
};
