import repo from '../Repository';
import { checkedProjects } from '../reducers/mainReducer';
import { updateProjectList } from './projectListActions';
import { clearCheckedProjects, showModalProgress } from './uiFlagsActions';

export const deleteProject = () => async (dispatch, getState) => {

    dispatch(showModalProgress(true));

    try {
        await repo.deleteProjects(checkedProjects(getState()));
        const data = await repo.loadProjects();
        dispatch(updateProjectList(data));
        // the projects won't be there anymore (hopefully)
        dispatch(clearCheckedProjects());
        dispatch(showModalProgress(false));
    } catch (e) {
        dispatch(showModalProgress(false));
        alert("Failed to delete project(s)");
    }
};