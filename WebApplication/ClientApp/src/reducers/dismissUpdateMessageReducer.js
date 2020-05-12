import mainActionTypes from '../actions/dismissUpdateMessageActions';
import projectListActionTypes from '../actions/projectListActions';

export default function(state = false, action) {
    switch(action.type) {
        case mainActionTypes.DISMISS_UPDATEMSG:
            return true;
        case projectListActionTypes.PARAMETER_EDITED:
            return false;
        case projectListActionTypes.PARAMETERS_RESET:
            return false;
        default:
           return state;
    }
}
