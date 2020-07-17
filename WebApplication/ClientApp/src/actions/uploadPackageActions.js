import repo from '../Repository';
import { uploadPackageData } from '../reducers/mainReducer';
import { addProject } from './projectListActions';
import { setProjectAlreadyExists, showUploadPackage } from './uiFlagsActions';
import { addError, addLog } from './notificationActions';
import { Jobs } from '../JobManager';

const actionTypes = {
    SET_UPLOAD_PROGRESS_VISIBLE: 'SET_UPLOAD_PROGRESS_VISIBLE',
    SET_UPLOAD_PROGRESS_HIDDEN: 'SET_UPLOAD_PROGRESS_HIDDEN',
    SET_UPLOAD_PROGRESS_DONE: 'SET_UPLOAD_PROGRESS_DONE',
    SET_UPLOAD_FAILED: 'SET_UPLOAD_FAILED',
    HIDE_UPLOAD_FAILED: 'HIDE_UPLOAD_FAILED'
};

export default actionTypes;

export const uploadPackage = () => async (dispatch, getState) => {
    const packageData = uploadPackageData(getState());

    if (packageData.file !== null && (packageData.root.length > 0 || packageData.file?.name.endsWith('.zip') === false)) {
        dispatch(showUploadPackage(false));
        dispatch(setUploadProgressVisible());

        let uploadResponse = null;

        try {
            uploadResponse = await repo.uploadPackage(packageData);
        } catch (e) {
            dispatch(setUploadProgressHidden());

            const httpStatus = e.response.status;
            if (httpStatus === 409) {
                dispatch(setProjectAlreadyExists(true));
            } else {
                const reportUrl = (httpStatus === 422) ? e.response.data.reportUrl : null;  // <<<---- the major change
                dispatch(setUploadFailed(reportUrl));
            }
        }

        const jobManager = Jobs();
        try {
            await jobManager.doAdoptJob(uploadResponse,
                // start job
                () => {
                    dispatch(addLog('JobManager: HubConnection started for adopt project : ' + uploadResponse));
                },
                // onComplete
                newProject => {
                    dispatch(addLog('JobManager: Received onComplete'));
                    dispatch(addProject(newProject));
                    dispatch(setUploadProgressDone());
                },
                // onError
                (jobId, reportUrl) => {
                    dispatch(addLog('JobManager: Received onError with jobId: ' + jobId + ' and reportUrl: ' + reportUrl));
                    dispatch(setUploadProgressHidden());
                    // show error modal dialog
                    dispatch(setUploadFailed(reportUrl));
                }
            );
        } catch (error) {
            dispatch(addError('JobManager: Error : ' + error));
        }
    }
};

export const setUploadProgressVisible = () => {
    return {
        type: actionTypes.SET_UPLOAD_PROGRESS_VISIBLE
    };
};

export const setUploadProgressHidden = () => {
    return {
        type: actionTypes.SET_UPLOAD_PROGRESS_HIDDEN
    };
};

export const setUploadProgressDone = () => {
    return {
        type: actionTypes.SET_UPLOAD_PROGRESS_DONE
    };
};

export const setUploadFailed = (reportUrl) => {
    return {
        type: actionTypes.SET_UPLOAD_FAILED,
        reportUrl
    };
};

export const hideUploadFailed = () => {
    return {
        type: actionTypes.HIDE_UPLOAD_FAILED
    };
};
