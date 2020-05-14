import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const getParameters = function(projectId, state) {
    return state[projectId];
};

export default function(state = initialState, action) {

    switch(action.type) {
        case parameterActionTypes.PARAMETERS_UPDATED: {
            const newState = { ...state };
            newState[action.projectId] = action.parameters;
            return newState;
        }

        case parameterActionTypes.PARAMETER_EDITED: {
            /* replace only the one parameter for the given project */
            const paramSet = (state[action.projectId]).map( (param) => param.name === action.parameter.name ? action.parameter : param);
            const newState = { ...state };
            newState[action.projectId] = paramSet;
            return newState;
        }

        case parameterActionTypes.PARAMETERS_RESET: {
            const newState = { ...state };
            newState[action.projectId] = action.parameters;
            return newState;
        }

        default:
            return state;
    }
}
