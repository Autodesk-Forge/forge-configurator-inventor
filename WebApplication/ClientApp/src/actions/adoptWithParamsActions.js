import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';
import { showAdoptWithParametersProgress, updateActiveTabIndex } from './uiFlagsActions';
import { updateActiveProject } from '../actions/projectListActions';
import { addProject } from './projectListActions';
//import { adaptParameters, updateParameters } from './parametersActions';

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
            (newProject) => {
                dispatch(addLog('JobManager: Adopt project with paramscReceived onComplete'));
                // hide modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                dispatch(addProject(newProject));
                //const adaptedParams = adaptParameters(parameters);
                //dispatch(updateParameters(newProject.id, adaptedParams));
                dispatch(updateActiveTabIndex(1));
                dispatch(updateActiveProject(newProject.id));
            },
            // onError
            (jobId, reportUrl) => {
                dispatch(addLog('JobManager: Adopt project with params Received onError reportUrl: ' + reportUrl));
                // hide progress modal dialog
                dispatch(showAdoptWithParametersProgress(false));
                // TODO: show error modal dialog
            }
        );
    } catch (error) {
        dispatch(addError('JobManager: Error : ' + error));
    }
};