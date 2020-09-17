/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showModalProgress, showUpdateFailed, setReportUrlLink, setStats } from './uiFlagsActions';

import { updateProject } from './projectListActions';

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
            label: param.label || key,
            readonly: !! param.readonly
        };
    });
}

export const fetchParameters = (projectId) => async (dispatch, getState) => {
    if (!projectId)
        return;
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

    // launch progress dialog immediately before we started connection to the server
    dispatch(showModalProgress(true));

    try {
        await jobManager.doUpdateJob(projectId, invFormattedParameters,
            // start job
            () => {
                dispatch(addLog('JobManager: HubConnection started for project : ' + projectId));
                dispatch(setReportUrlLink(null)); // cleanup url link
            },
            // onComplete
            (updatedState, stats) => {
                dispatch(addLog('JobManager: Received onComplete'));
                dispatch(setStats(stats));

                // parameters and "base project state" should be handled differently,
                // so split the incoming updated state to pieces.
                const { parameters, ...baseProjectState } = updatedState;

                // launch update
                const adaptedParams = adaptParameters(parameters);
                dispatch(updateParameters(projectId, adaptedParams));
                dispatch(updateProject(projectId, baseProjectState));
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Received onError with jobId: ' + jobId + ' and reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showModalProgress(false));
                // show error modal dialog
                dispatch(setReportUrlLink(reportUrl));
                dispatch(showUpdateFailed(true));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager: Error : ' + error));
    }
};

const stripUnits = (parameter) => {
    if(!parameter.value.endsWith(parameter.units)) {
        return parameter.value.trim();
    }

    const stripped = parameter.value.slice(0, parameter.value.length - parameter.units.length - 1);
    return stripped.trim();
};

// Compares the two parameter values and return true if they represent the same value
export const compareParamaters = (firstParameter, secondParameter) => {
    if(!firstParameter || !secondParameter) {
        return false;
    }

    if(firstParameter.value === secondParameter.value) {
        return true;
    }

    if(firstParameter.units !== secondParameter.units) {
        // no units conversions
        return false;
    }

    // if not matched easily, try to strip out the units from both and recompare
    return (stripUnits(firstParameter) === stripUnits(secondParameter));
};
