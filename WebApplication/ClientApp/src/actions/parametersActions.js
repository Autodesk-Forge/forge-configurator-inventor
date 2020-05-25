import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showUpdateProgress } from './uiFlagsActions';
import { updateSvf } from './projectListActions';

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
export const fetchParameters = (projectId) => async (dispatch, getState) => {
    const params = getState().updateParameters[projectId];
    if(params && params.length!==0) {
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

export function formatParameters(clientParameters) {
    const quote = function(input) {
        if (input == null)
            return input;

        const out = "\"".concat(input, "\"");
        return out;
    };

    const invFormatedParameters = clientParameters.reduce( (obj, param) => {
        const quoteValues = param.allowedValues != null && param.allowedValues.length > 0;
        const values = quoteValues ? param.allowedValues.map(item => quote(item)) : [];
        const value = quoteValues ? quote(param.value) : param.value;

        obj[param.name] = {
            value: value,
            unit: param.units ? param.units : null,
            values: values
        };

        return obj;
    }, {});

    return invFormatedParameters;
}

export const updateModelWithParameters = (projectId, data) => async (dispatch) => {
    dispatch(addLog('updateModelWithParameters invoked'));

    // update 'data' parameters back to inventor format
    const invFormattedParameters = formatParameters(data);
    const jobManager = Jobs();

    try {
        await jobManager.doJob(projectId, invFormattedParameters,
            // start job
            () => {
                dispatch(addLog('JobManager: HubConnection started for project : ' + projectId));
                dispatch(showUpdateProgress(true));
            },
            // onComplete
            (_, updatedState) => {
                dispatch(addLog('JobManager: Received onComplete'));

                const rawParameters = updatedState.parameters;
                const svf = updatedState.svf;
                
                // hide modal dialog
                dispatch(showUpdateProgress(false));

                // launch update
                const parameters = adaptParameters(rawParameters);
                dispatch(updateParameters(projectId, parameters));
                dispatch(updateSvf(projectId, svf));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager: Error : ' + error));
    }
};
