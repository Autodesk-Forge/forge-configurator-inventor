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
<<<<<<< HEAD
import { addProject } from './projectListActions';
import { adaptParameters, updateParameters } from './parametersActions';
=======
import { addOrUpdateProject } from './projectListActions';
>>>>>>> INVGEN-48194-finish-cpq-poc

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
            (project) => {
                dispatch(addLog('JobManager: Adopt project with paramscReceived onComplete'));

                // hide modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                dispatch(addProject(project));
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