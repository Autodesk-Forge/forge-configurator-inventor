import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const parametersReducer = function(state = initialState, action) {

    switch(action.type) {
        case parameterActionTypes.PARAMETERS_UPDATED: {
            /* should be something like...
            let newState = { ...state };
            newState[action.projectId] = action.parameters;
            */
            return newState;
        }

        case parameterActionTypes.PARAMETER_EDITED: // do nothing here!
        case parameterActionTypes.PARAMETERS_RESET: // do nothing here!
        default:
            return state;
    }
};
