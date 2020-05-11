import projectListActionTypes from '../actions/projectListActions';

export const initialState = {
    activeProjectId: null
};

export const getActiveProject = function(state) {
    if (! state.projects) return undefined;
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
            let activeProject = action.projectList.find(({ id }) => id === state.activeProjectId);
            var prj = activeProject ? activeProject : (action.projectList.length ? action.projectList[0] : null);
            const prjId = prj ? prj.id : null;
            return { activeProjectId: prjId, projects: action.projectList};
        }
        case projectListActionTypes.ACTIVE_PROJECT_UPDATED: {
            return { ...state, activeProjectId: action.activeProjectId};
        }
        case projectListActionTypes.PARAMETERS_UPDATED: {
            let projects = state.projects.map((project) => {
                return project.id !== action.projectId ? project : {
                  ...project,
                  parameters: action.parameters,
                  updateParameters: action.parameters
                };
            });

            return { ...state, projects: projects };
        }
        case projectListActionTypes.PARAMETER_EDITED: {
            let projects = state.projects.map((project) => {
                return project.id !== action.projectId ? project : {
                  ...project,
                  updateParameters: project.updateParameters.map((param) => {
                      /* for now, allowing to change only value during edit */
                      return param.name !== action.parameter.name ? param : {
                          ...param,
                          value: action.parameter.value
                      };
                  })
                };
            });

            return { ...state, projects: projects };
        }
        case projectListActionTypes.PARAMETERS_RESET: {
            let projects = state.projects.map((project) => {
                return project.id !== action.projectId ? project : {
                  ...project,
                  updateParameters: project.parameters
                };
            });

            return { ...state, projects: projects };
        }
        default:
            return state;
    }
};