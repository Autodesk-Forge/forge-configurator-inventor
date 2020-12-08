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
import { showDownloadProgress, showDownloadFailed, setDownloadLink, setErrorData, setStats, setReportUrl, hideDownloadProgress } from './uiFlagsActions';
import { showDrawingExportProgress, setDrawingPdfUrl } from './uiFlagsActions';

/**
 * Generic method to handle generation of downloads. The following happens:
 * - show 'in progress' dialog
 * - call SignalR method to generate downloads
 * - wait for completion
 * - store either 'download url' (for success), or 'report url' (for failure)
 * - show Succeeded or Failed dialog
 *
 * @param {string} methodName  SignalR method to call.
 * @param {string} projectId   Project ID. (passed as a first arg to the SignalR method)
 * @param {string} hash        Parameters hash. (passed as a second arg to the SignalR method)
 * @param {string} dialogTitle Title for dialogs.
 */
export const getDownloadLink = (methodName, projectId, hash, dialogTitle, key) => async (dispatch) => {
    dispatch(addLog(`getDownloadLink invoked for ${methodName}`));

    const jobManager = Jobs();

    // show progress
    dispatch(showDownloadProgress(true, dialogTitle));

    // launch signalR to generate download and wait for result
    try {
        await jobManager.doDownloadJob(methodName, projectId, hash, key,
            // start job
            () => {
                dispatch(addLog(`JobManager.doDownloadJob: '${methodName}' started for project : ${projectId}`));
                dispatch(setErrorData(null)); // cleanup url link
            },
            // onComplete
            (downloadUrl, stats, reportUrl) => {
                dispatch(addLog(`JobManager.doDownloadJob: '${methodName}' completed for project : ${projectId}`));
                // set download link, it will show link in UI
                dispatch(setDownloadLink(downloadUrl));
                dispatch(setStats(stats));
                dispatch(setReportUrl(reportUrl));
            },
            // onError
            (errorData) => {
                dispatch(addLog('JobManager.doDownloadJob: Received onError with jobId: ' + errorData.jobId));
                // hide progress modal dialog
                dispatch(hideDownloadProgress());
                // show error modal dialog
                dispatch(setErrorData(errorData));
                dispatch(showDownloadFailed(true));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager.doDownloadJob: Error : ' + error));
    }
};

export const fetchDrawing = (project, drawingKey) => async (dispatch) => {
    if (! project.id || !drawingKey) return;

    dispatch(addLog('fetchDrawing invoked'));

    const jobManager = Jobs();

    // show progress
    dispatch(showDrawingExportProgress(true));

    // launch signalR to export drawing and wait for result
    try {
        await jobManager.doDrawingExportJob(project.id, project.hash, drawingKey,
            // start job
            () => {
                dispatch(addLog('JobManager.doDrawingExportJob: HubConnection started for project : ' + project.id + ' (drawing: ' + drawingKey + ')'));
            },
            // onComplete
            (drawingPdfUrl, stats, reportUrl) => {
                dispatch(addLog('JobManager.doDrawingExportJob: Received onComplete'));
                // store drawings link
                dispatch(setDrawingPdfUrl(drawingKey, drawingPdfUrl));
                dispatch(setStats(stats, drawingKey));
                dispatch(setReportUrl(reportUrl));
            },
            // onError
            (errorData) => {
                dispatch(addLog('JobManager: Received onError with jobId: ' + errorData.jobId));
                // hide progress modal dialog
                dispatch(showDrawingExportProgress(false));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager.doDrawingExportJob: Error : ' + error));
    }
};