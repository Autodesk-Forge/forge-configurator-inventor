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
