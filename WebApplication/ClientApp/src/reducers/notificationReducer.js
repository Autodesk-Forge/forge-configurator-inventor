import notificationTypes from '../actions/notificationActions';

export const initialState = [
    '0 Errors'
]

export const notificationReducer = function(state = initialState, action) {
    switch(action.type) {
        case notificationTypes.ADD_ERROR: {
            return ([ ...state, action.info ]);
        }
        case notificationTypes.ADD_LOG: {
            return ([ ...state, action.info ]);
        }        
        default:
            return state;
    }
}