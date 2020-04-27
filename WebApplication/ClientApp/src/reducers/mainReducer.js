import {combineReducers} from 'redux';
import {projectListReducer, getActiveProject as listReducerActiveProject } from './projectListReducers';
import {notificationReducer} from './notificationReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer
});

export const getActiveProject = function(state) {
    return listReducerActiveProject(state.projectList);
}