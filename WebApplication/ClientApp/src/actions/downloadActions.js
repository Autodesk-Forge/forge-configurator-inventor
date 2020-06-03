import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showRFAModalProgress, setRFALink } from './uiFlagsActions';

export const getRFADownloadLink = (projectId, temporaryUrl) => async (dispatch) => {
    dispatch(addLog('getRFADownloadLink invoked'));

    const jobManager = Jobs();

    // show progress
    dispatch(showRFAModalProgress(projectId));

    // launch signalR to make RFA here and wait for result
    try {
        await jobManager.doRFAJob(projectId, temporaryUrl,
            // start job
            () => {
                dispatch(addLog('JobManager.doRFAJob: HubConnection started for project : ' + projectId));
            },
            // onComplete
            (rfaUrl) => {
                dispatch(addLog('JobManager.doRFAJob: Received onComplete'));
                // set RFA link, it will show link in UI
                dispatch(setRFALink(rfaUrl));
            }
        );
    } catch (error) {
        dispatch(addError('JobManager.doRFAJob: Error : ' + error));
    }
};
