import {combineReducers} from 'redux';
import {projectListReducer, getActiveProject as listReducerActiveProject, getProject as listReducerGetProject } from './projectListReducers';
import {notificationReducer} from './notificationReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer
});

export const getActiveProject = function(state) {
    return listReducerActiveProject(state.projectList);
};

export const getProject = function(id, state) {
    return listReducerGetProject(id, state.projectList);
};