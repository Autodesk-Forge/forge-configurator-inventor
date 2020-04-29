import projectListActionTypes from '../actions/projectListActions';

export const initialState = {
    activeProjectId: null
}

export const getActiveProject = function(state) {
    return state.projects?.find(proj => proj.id === state.activeProjectId)
}

export const projectListReducer = function(state = initialState, action) {
    switch(action.type) {
        case projectListActionTypes.PROJECT_LIST_UPDATED: {
            // select the previous active project if present, or first project otherwise
            let activeProject = action.projectList.find(({ id }) => id === state.activeProjectId);
            var prj = activeProject ? activeProject : (action.projectList.length ? action.projectList[0] : null);
            const prjId = prj ? prj.id : null;
            return { activeProjectId: prjId, projects: action.projectList};
        }
        case projectListActionTypes.ACTIVE_PROJECT_UPDATED: {
            return { ...state, activeProjectId: action.activeProjectId};
        }
        case projectListActionTypes.PARAMETERS_UPDATED: {
            var projects = state.projects.map((project) => {
                return project.id !== action.projectId ? project : {
                  ...project,
                  parameters: action.parameters
                }
            });

            return { ...state, projects: projects };
        }
        default:
            return state;
    }
}