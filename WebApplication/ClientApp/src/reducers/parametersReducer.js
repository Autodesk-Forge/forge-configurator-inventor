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

        case parameterActionTypes.PARAMETER_EDITED: // do nothing here!
        case parameterActionTypes.PARAMETERS_RESET: // do nothing here!
        default:
            return state;
    }
}
