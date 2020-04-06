import projectListActionTypes from '../actions/projectListActions';

export const initialState = {
    activeProjectId: null
}

export const projectListReducer = function(state = initialState, action) {
    switch(action.type) {
        case projectListActionTypes.PROJECT_LIST_UPDATED: {
            // select the previous active project if present, or first project otherwise
            let activeProject = action.projectList.find( ({id}) => id === state.activeProjectId);
            const prjId = activeProject ? activeProject.id : (action.projectList.length ? action.projectList[0].id : null);
            return { activeProjectId: prjId, projects: action.projectList};
        }
        case projectListActionTypes.ACTIVE_PROJECT_UPDATED: {
            return { ...state, activeProjectId: action.activeProjectId};
        }
        default:
            return state;
    }
}