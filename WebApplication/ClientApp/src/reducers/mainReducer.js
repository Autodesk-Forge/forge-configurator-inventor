import {combineReducers} from 'redux';
import {projectListReducer, getActiveProject as listReducerActiveProject, getProject as listReducerGetProject } from './projectListReducers';
import {notificationReducer} from './notificationReducer';
import {parametersReducer} from './parametersReducer';
import {updateParametersReducer} from './updateParametersReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer,
    parameters: parametersReducer,
    updateParameters: updateParametersReducer
});

export const getActiveProject = function(state) {
    return listReducerActiveProject(state.projectList);
};

export const getProject = function(id, state) {
    return listReducerGetProject(id, state.projectList);
};