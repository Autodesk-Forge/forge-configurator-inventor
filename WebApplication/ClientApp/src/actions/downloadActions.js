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
import { showRFAModalProgress, showRfaFailed, setRFALink, setReportUrlLink } from './uiFlagsActions';

export const getRFADownloadLink = (projectId, temporaryUrl) => async (dispatch) => {
    dispatch(addLog('getRFADownloadLink invoked'));

    const jobManager = Jobs();

    // show progress
    dispatch(showRFAModalProgress(true));

    // launch signalR to make RFA here and wait for result
    try {
        await jobManager.doRFAJob(projectId, temporaryUrl,
            // start job
            () => {
                dispatch(addLog('JobManager.doRFAJob: HubConnection started for project : ' + projectId));
                dispatch(setReportUrlLink(null)); // cleanup url link
            },
            // onComplete
            (rfaUrl) => {
                dispatch(addLog('JobManager.doRFAJob: Received onComplete'));
                // set RFA link, it will show link in UI
                dispatch(setRFALink(rfaUrl));
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Received onError with jobId: ' + jobId + ' and reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showRFAModalProgress(false));
                // show error modal dialog
                dispatch(setReportUrlLink(reportUrl));
                dispatch(showRfaFailed(true));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager.doRFAJob: Error : ' + error));
    }
};
