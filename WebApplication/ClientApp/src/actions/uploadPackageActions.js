import repo from '../Repository';
import { uploadPackageData } from '../reducers/mainReducer';
import { addProject } from './projectListActions';
import { setProjectAlreadyExists, showUploadPackage } from './uiFlagsActions';

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

        try {
            const newProject = await repo.uploadPackage(packageData);
            dispatch(addProject(newProject));
            dispatch(setUploadProgressDone());
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
