import repo from '../Repository';
import {uploadPackageData} from '../reducers/mainReducer';

const actionTypes = {
    SHOW_UPLOAD_PROGRESS: 'SHOW_UPLOAD_PROGRESS'
};

export default actionTypes;

export const uploadPackage = () => async (dispatch, getState) => {
    dispatch(showUploadProgress());
    repo.uploadPackage(uploadPackageData(getState()));
    dispatch(showUploadProgress("#done"));
};

export const showUploadProgress = (status) => {
    return {
        type: actionTypes.SHOW_UPLOAD_PROGRESS,
        status
    };
};
