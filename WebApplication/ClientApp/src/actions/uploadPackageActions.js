import repo from '../Repository';
import { uploadPackageData } from '../reducers/mainReducer';
import { addProject } from './projectListActions';
import { setProjectAlreadyExists } from './uiFlagsActions';

const actionTypes = {
    SET_UPLOAD_PROGRESS_VISIBLE: 'SET_UPLOAD_PROGRESS_VISIBLE',
    SET_UPLOAD_PROGRESS_HIDDEN: 'SET_UPLOAD_PROGRESS_HIDDEN',
    SET_UPLOAD_PROGRESS_DONE: 'SET_UPLOAD_PROGRESS_DONE'
};

export default actionTypes;

export const uploadPackage = () => async (dispatch, getState) => {

    dispatch(setUploadProgressVisible());

    try {
        const newProject = await repo.uploadPackage(uploadPackageData(getState()));
        dispatch(addProject(newProject));
        dispatch(setUploadProgressDone());
    } catch (e) {
        dispatch(setUploadProgressHidden());
        if (e.response.status === 409) {
            dispatch(setProjectAlreadyExists(true));
        } else {
            alert("Failed to adopt the project");
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
