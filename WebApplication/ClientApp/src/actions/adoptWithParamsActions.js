import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showModalProgress, showAdoptWithParametersProgress } from './uiFlagsActions';

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
            () => {
                dispatch(addLog('JobManager: Received onComplete'));
                // hide modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                // TODO: Show done for adopt
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Received onError reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                // show error modal dialog
            }
        );
    } catch (error) {
        dispatch(addError('JobManager: Error : ' + error));
    }
};