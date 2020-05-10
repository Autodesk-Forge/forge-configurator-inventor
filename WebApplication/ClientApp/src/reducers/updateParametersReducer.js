import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const updateParametersReducer = function(state = initialState, action) {

    switch(action.type) {
        case parameterActionTypes.PARAMETERS_UPDATED: {
            /* should be something like ...
            let newState = { ...state };
            newState[action.projectId] = action.parameters;
            */
            return newState;
        }

        case parameterActionTypes.PARAMETER_EDITED: {
            /* replace only the one parameter for the given project */
            return newState;
        }
        case parameterActionTypes.PARAMETERS_RESET: {
            /*TBD*/

            return state;
        }
        default:
            return state;
    }
};
