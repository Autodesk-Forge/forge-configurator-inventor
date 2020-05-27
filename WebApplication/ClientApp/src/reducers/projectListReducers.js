import projectListActionTypes from '../actions/projectListActions';

export const initialState = {
    activeProjectId: null
};

export const getActiveProject = function(state) {
    // when no projects available, returns empty project for correct UI initialization
    if (! state.projects) return { };
    return getProject(state.activeProjectId, state);
};

export const getProject = function(id, state) {
    if (! state.projects) return undefined;
    return state.projects.find(proj => proj.id === id);
};

export default function(state = initialState, action) {
    switch(action.type) {
        case projectListActionTypes.PROJECT_LIST_UPDATED: {
            // select the previous active project if present, or first project otherwise
            const activeProject = action.projectList.find(({ id }) => id === state.activeProjectId);
            const prj = activeProject ? activeProject : (action.projectList.length ? action.projectList[0] : null);
            const prjId = prj ? prj.id : null;
            return { activeProjectId: prjId, projects: action.projectList};
        }
        case projectListActionTypes.ACTIVE_PROJECT_UPDATED: {
            return { ...state, activeProjectId: action.activeProjectId};
        }
        case projectListActionTypes.UPDATE_SVF: {
            const projects = state.projects.map((project) => {
                return project.id !== action.activeProjectId ? project : {
                    ...project, svf: action.svf
                };
            });
            return { ...state, projects };
        }
        default:
            return state;
    }
}