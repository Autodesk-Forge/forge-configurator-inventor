import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const parameterReducer = function(state = initialState, action) {

    switch(action.type) {

        case parameterActionTypes.PARAMETERS_UPDATED: {
            const projects = state.projects.map((project) => {
                return project.id !== action.projectId ? project : {
                ...project,
                parameters: action.parameters,
                updateParameters: action.parameters
                };
            });

            return { ...state, projects: projects };
        }

        case parameterActionTypes.PARAMETER_EDITED: {
            const projects = state.projects.map((project) => {
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

        case parameterActionTypes.PARAMETERS_RESET: {
            const projects = state.projects.map((project) => {
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
