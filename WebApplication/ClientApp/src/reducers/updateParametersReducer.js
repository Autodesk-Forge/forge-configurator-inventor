import parameterActionTypes from "../actions/parametersActions";

export const initialState = {};

export const getParameters = function(projectId, state) {
    return state[projectId];
};

export default function(state = initialState, action) {

    switch(action.type) {
        case parameterActionTypes.PARAMETERS_UPDATED: {
            const prevState = state[action.projectId];
            let paramSet = action.parameters;
            if(prevState) {
                // compare the new values to previous ones and mark those that have changed during the update
                // no need to clear the changedOnUpdate if it was there as it is not part of the newly returned data
                paramSet = action.parameters.map( (param) => param.value == prevState.find( (elem) => elem.name == param.name)?.value ? param : { ...param, changedOnUpdate: true } );
            }
            const newState = { ...state };
            newState[action.projectId] = paramSet;
            return newState;
        }

        case parameterActionTypes.PARAMETER_EDITED: {
            /* replace only the one parameter for the given project */
            const paramSet = (state[action.projectId]).map( (param) => param.name === action.parameter.name ? { ...param, value: action.parameter.value, changedOnUpdate: null } : param);
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
