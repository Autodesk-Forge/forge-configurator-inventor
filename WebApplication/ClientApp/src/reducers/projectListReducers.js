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

            // hack parameters now
            prj.parameters = [
                {
                    name: 'param 1',
                    value: "v1",
                    type: "string",
                    units: ""
                },
                {
                    name: 'param 2',
                    value: "22",
                    type: "number",
                    units: "in"
                },
                {
                    name: 'param 3',
                    value: "true",
                    type: "boolean",
                    units: ""
                },
                {
                    name: 'param 4',
                    value: "v4",
                    type: "number",
                    units: "in"
                }];

            const prjId = prj ? prj.id : null;
            return { activeProjectId: prjId, projects: action.projectList};
        }
        case projectListActionTypes.ACTIVE_PROJECT_UPDATED: {
            return { ...state, activeProjectId: action.activeProjectId};
        }
        default:
            return state;
    }
}