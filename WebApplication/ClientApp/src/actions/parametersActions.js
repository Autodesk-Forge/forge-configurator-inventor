import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showUpdateProgress } from './uiFlagsActions';

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

export const resetParameters = (projectId, parameters) => {
    return {
        type: actionTypes.PARAMETERS_RESET,
        projectId,
        parameters
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
    const unquote = function(input) {
        if (input == null || input.length < 2)
            return input;

        if (input[0] === "\"" && input[input.length-1] === "\"")
            return input.substr(1, input.length-2);

        return input;
    };

    return Object.entries(rawParameters).map( ([key, param]) => {
        return {
            name: key,
            value: unquote(param.value),
            allowedValues: (param.values) ? param.values.map( item => unquote(item)) : [],
            units: param.unit,
            type: "NYI" // TODO: remove?
        };
    });
}

// eslint-disable-next-line no-unused-vars
export const fetchParameters = (projectId, force = false) => async (dispatch, getState) => {
    const params = getState().updateParameters[projectId];
    if(!force && params && params.length!==0) {
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

export const updateModelWithParameters = (projectId, data) => async (dispatch) => {
    dispatch(addLog('updateModelWithParameters invoked'));

    const jobManager = Jobs();
    jobManager.doJob(projectId, data,
        // start job
        () => {
            // launch modal dialog
        },
        // onComplete
        (jobId) => {
            // hide modal dialog
            dispatch(showUpdateProgress(false));

            // just get rid of lint warning. jobId will be used later
            const job = jobId;
            jobId = job;

            // launch some update here
        }
    );
};
