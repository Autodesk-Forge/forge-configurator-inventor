import * as projectListActions from '../actions/projectListActions';

const initialState = [
    {
        id: '1',
        label: 'Local Project 1',
        image: 'bike.png'
    },
    {
        id: '2',
        label: 'Another Local One',
        image: 'log.png'
    }
]

export const projectListReducer = function(state = initialState, action) {
    switch(action.type) {
        case projectListActions.PROJECT_LIST: {
            action.projectList = action.projectList || [];
            // always do complete replacement for this action
            return action.projectList;
        }
        default:
            return state;
    }
}