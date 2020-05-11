import {combineReducers} from 'redux';
import projectListReducer, * as list from './projectListReducers';
import {notificationReducer} from './notificationReducer';
import {parametersReducer} from './parametersReducer';
import {updateParametersReducer} from './updateParametersReducer';
import dismissUpdateMessageReducer from './dismissUpdateMessageReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer,
    parameters: parametersReducer,
    updateParameters: updateParametersReducer,
    dismissUpdateMessage: dismissUpdateMessageReducer
});

export const getActiveProject = function(state) {
    return list.getActiveProject(state.projectList);
};

export const getProject = function(id, state) {
    return list.getProject(id, state.projectList);
};

export const showUpdateNotification = function(state) {
    if (state.dismissUpdateMessage === true)
        return false;

    const activeProject = getActiveProject(state);

    if (!activeProject.parameters)
        return false;

    for (const parameterId in activeProject.parameters) {
        const parameter = activeProject.parameters[parameterId];
        const updateParameter = activeProject.updateParameters.find(updatePar => updatePar.name === parameter.name);
        if (parameter.value !== updateParameter.value) {
            return true;
        }
    }

    return false;
};
