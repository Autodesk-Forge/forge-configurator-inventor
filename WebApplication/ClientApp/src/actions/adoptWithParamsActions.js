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

import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showAdoptWithParamsFailed, showAdoptWithParametersProgress, updateActiveTabIndex } from './uiFlagsActions';
import { updateActiveProject } from '../actions/projectListActions';
import { addOrUpdateProject } from './projectListActions';
import { adaptParameters, updateParameters } from './parametersActions';

export const adoptProjectWithParameters = (parameters) => async (dispatch) => {
    dispatch(addLog('adoptProjectWithParameters invoked'));

    const jobManager = Jobs();

    // launch progress dialog immediately before we started connection to the server
    dispatch(showAdoptWithParametersProgress(true));

    try {
        await jobManager.doAdoptWithParameters(parameters,
            // start job
            () => {
                dispatch(addLog('JobManager: HubConnection started for adopt project with params'));
            },
            // onComplete
            (projectWithParams) => {
                dispatch(addLog('JobManager: Adopt project with paramscReceived onComplete'));

                const project = projectWithParams.project;
                const projectUpdate = projectWithParams.parameters;
                const params = projectWithParams.parameters.parameters;

                // Adapt the incoming update directly to our model
                project.svf = projectUpdate.svf;
                project.bomDownloadUrl = projectUpdate.bomDownloadUrl;
                project.bomJsonUrl = projectUpdate.bomJsonUrl;
                project.modelDownloadUrl = projectUpdate.modelDownloadUrl;
                project.hash = projectUpdate.hash;

                // hide modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                dispatch(addOrUpdateProject(project));
                const adaptedParams = adaptParameters(params);
                dispatch(updateParameters(project.id, adaptedParams));
                dispatch(updateActiveProject(project.id));
                dispatch(updateActiveTabIndex(0));
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Adopt project with params Received onError reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                dispatch(showAdoptWithParamsFailed(true));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager: Error : ' + error));
    }
};