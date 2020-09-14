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
            (projectwithparams) => {
                dispatch(addLog('JobManager: Adopt project with paramscReceived onComplete'));

                const project = projectwithparams.project;
                const projectUpdate = projectwithparams.parameters;
                const params = projectwithparams.parameters.parameters;

                // Adapt the incoming update directly to our model
                project.svf = projectUpdate.svf;
                project.bomDownloadUrl = projectUpdate.bomDownloadUrl;
                project.bomJsonUrl = projectUpdate.bomJsonUrl;
                project.modelDownloadUrl = projectUpdate.modelDownloadUrl;
                project.hash = projectUpdate.hash;
                project.isAssembly = projectUpdate.isAssembly;
                project.hasDrawing = projectUpdate.hasDrawing;

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