import {combineReducers} from 'redux';
import projectListReducer, * as list from './projectListReducers';
import {notificationReducer} from './notificationReducer';
import dismissUpdateMessageReducer from './dismissUpdateMessageReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer,
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

    let activeProject = getActiveProject(state);

    if (!activeProject.parameters)
        return false;

    for (let parameterId in activeProject.parameters) {
        let parameter = activeProject.parameters[parameterId];
        let updateParameter = activeProject.updateParameters.find(updatePar => updatePar.name === parameter.name)
        if (parameter.value !== updateParameter.value) {
            return true;
        }
    }

    return false;
}
