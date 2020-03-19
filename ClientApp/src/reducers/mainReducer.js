import {combineReducers} from 'redux';
import {projectListReducer} from './projectListReducers';

export const mainReducer = combineReducers({
    projectList: projectListReducer
});