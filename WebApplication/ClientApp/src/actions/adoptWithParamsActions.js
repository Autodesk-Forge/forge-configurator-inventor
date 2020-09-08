import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showModalProgress, setReportUrlLink } from './uiFlagsActions';

export const adoptProjectWithParameters = (parameters) => async (dispatch) => {
    dispatch(addLog('adoptProjectWithParameters invoked'));

    const jobManager = Jobs();

    // launch progress dialog immediately before we started connection to the server
    dispatch(showModalProgress(true));

    try {
        await jobManager.doAdoptWithParameters(parameters,
            // start job
            () => {
                dispatch(addLog('JobManager: HubConnection started'));
                dispatch(setReportUrlLink(null)); // cleanup url link
            },
            // onComplete
            updatedState => {
                dispatch(addLog('JobManager: Received onComplete'));
                // hide modal dialog
                dispatch(showModalProgress(false));
                // TODO: Show done for adopt
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Received onError reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showModalProgress(false));
                // show error modal dialog
                dispatch(setReportUrlLink(reportUrl));
                // TODO: Show adopt failed and error
            }
        );
    } catch (error) {
        dispatch(addError('JobManager: Error : ' + error));
    }
};