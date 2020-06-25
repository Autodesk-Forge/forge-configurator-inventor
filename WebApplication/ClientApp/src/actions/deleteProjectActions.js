import repo from '../Repository';
import { checkedProjects } from '../reducers/mainReducer';
import { updateProjectList } from './projectListActions';

export const deleteProject = () => async (dispatch, getState) => {

    // dispatch(setUploadProgressVisible());

    try {
        await repo.deleteProjects(checkedProjects(getState()));
        const data = await repo.loadProjects();
        dispatch(updateProjectList(data));
        // dispatch(setUploadProgressDone());
    } catch (e) {
        // dispatch(setUploadProgressHidden());
        alert("Failed to delete project(s)");
    }
};