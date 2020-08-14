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
import { showDrawingDownloadModalProgress, showDrawingDownloadFailed, setDrawingDownloadLink } from './uiFlagsActions';
import { showDrawingExportProgress, setDrawingPdfUrl } from './uiFlagsActions';

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

export const getDrawingDownloadLink = (projectId, temporaryUrl) => async (dispatch) => {
    dispatch(addLog('getDrawingDownloadLink invoked'));

    const jobManager = Jobs();

    // show progress
    dispatch(showDrawingDownloadModalProgress(true));

    // launch signalR to prepare up-to-date drawing here and wait for result
    try {
        await jobManager.doDrawingDownloadJob(projectId, temporaryUrl,
            // start job
            () => {
                dispatch(addLog('JobManager.doDrawingDownloadJob: HubConnection started for project : ' + projectId));
                dispatch(setReportUrlLink(null)); // cleanup url link
            },
            // onComplete
            (drawingUrl) => {
                dispatch(addLog('JobManager.doDrawingDownloadJob: Received onComplete'));
                // set RFA link, it will show link in UI
                dispatch(setDrawingDownloadLink(drawingUrl));
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Received onError with jobId: ' + jobId + ' and reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showDrawingDownloadModalProgress(false));
                // show error modal dialog
                dispatch(setReportUrlLink(reportUrl));
                dispatch(showDrawingDownloadFailed(true));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager.doDrawingDownloadJob: Error : ' + error));
    }
};

export const fetchDrawing = (project) => async (dispatch) => {
    if (! project.id) return;

    dispatch(addLog('fetchDrawing invoked'));

    const jobManager = Jobs();

    // show progress
    dispatch(showDrawingExportProgress(true));

    // launch signalR to export drawing and wait for result
    try {
        await jobManager.doDrawingExportJob(project.id, project.hash,
            // start job
            () => {
                dispatch(addLog('JobManager.doDrawingExportJob: HubConnection started for project : ' + project.id));
                //dispatch(setReportUrlLink(null)); // cleanup url link
            },
            // onComplete
            (drawingPdfUrl) => {
                dispatch(addLog('JobManager.doDrawingExportJob: Received onComplete'));
                // store drawings link
                dispatch(setDrawingPdfUrl(drawingPdfUrl));
                // hide progress modal dialog
                dispatch(showDrawingExportProgress(false));
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Received onError with jobId: ' + jobId + ' and reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showDrawingExportProgress(false));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager.doDrawingExportJob: Error : ' + error));
    }
};