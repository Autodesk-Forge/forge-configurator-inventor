import {combineReducers} from 'redux';
import {projectListReducer} from './projectListReducers';
import {notificationReducer} from './notificationReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer
});