import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const updateParametersReducer = function(state = initialState, action) {

    switch(action.type) {
        case parameterActionTypes.PARAMETERS_UPDATED: {
            let newState = { ...state };
            newState[action.projectId] = action.parameters;
            return newState;
        }

        case parameterActionTypes.PARAMETER_EDITED: {
            /* replace only the one parameter for the given project */
            let paramSet = (state[action.projectId]).map( (param) => param.name === action.parameter.name ? action.parameter : param);
            let newState = { ...state };
            newState[action.projectId] = paramSet;
            return newState;
        }
        
        case parameterActionTypes.PARAMETERS_RESET: {
            let newState = { ...state };
            newState[action.projectId] = action.parameters;
            return newState;
        }
        
        default:
            return state;
    }
};
