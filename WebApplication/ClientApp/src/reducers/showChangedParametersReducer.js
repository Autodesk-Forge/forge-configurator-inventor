import { actionTypes } from '../actions/applicationActions';

export default function(state = true, action) {
    switch(action.type) {
        case actionTypes.UPDATE_SHOW_PARAMETERS_CHANGED:
            return action.show;
        default:
           return state;
    }
}
