import {combineReducers} from 'redux';
import projectListReducer, * as list from './projectListReducers';
import {notificationReducer} from './notificationReducer';
import parametersReducer, * as params from './parametersReducer';
import updateParametersReducer, * as updateParams from './updateParametersReducer';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import profileReducer from './profileReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer,
    parameters: parametersReducer,
    updateParameters: updateParametersReducer,
    uiFlags: uiFlagsReducer,
    profile: profileReducer
});

export const getActiveProject = function(state) {
    return list.getActiveProject(state.projectList);
};

export const getProject = function(id, state) {
    return list.getProject(id, state.projectList);
};

export const getParameters = function(projectId, state) {
    return params.getParameters(projectId, state.parameters);
};

export const getUpdateParameters = function(projectId, state) {
    return updateParams.getParameters(projectId, state.updateParameters);
};

export const parametersEditedMessageVisible = function(state) {
    if (state.uiFlags.parametersEditedMessageClosed === true || state.uiFlags.parametersEditedMessageRejected === true )
        return false;

    const activeProject = getActiveProject(state);
    if (!activeProject)
        return false;

    const parameters = getParameters(activeProject.id, state);
    const updateParameters = getUpdateParameters(activeProject.id, state);

    if (!parameters || !updateParameters)
        return false;

    for (const parameterId in parameters) {
        const parameter = parameters[parameterId];
        const updateParameter = updateParameters.find(updatePar => updatePar.name === parameter.name);
        if (parameter.value !== updateParameter.value) {
            return true;
        }
    }

    return false;
};

export const modalProgressShowing = function(state) {
    return uiFlags.modalProgressShowing(state.uiFlags);
};

export const updateFailedShowing = function(state) {
    return uiFlags.updateFailedShowing(state.uiFlags);
};

export const rfaProgressShowing = function(state) {
    return uiFlags.rfaProgressShowing(state.uiFlags);
};

export const rfaDownloadUrl = function(state) {
    return uiFlags.rfaDownloadUrl(state.uiFlags);
};

export const uploadPackageDlgVisible = function(state) {
    return uiFlags.uploadPackageDlgVisible(state.uiFlags);
};

export const uploadProgressShowing = function(state) {
    return uiFlags.uploadProgressShowing(state.uiFlags);
};

export const uploadProgressIsDone = function(state) {
    return uiFlags.uploadProgressIsDone(state.uiFlags);
};

export const uploadPackageData = function(state) {
    return uiFlags.uploadPackageData(state.uiFlags);
};

export const getProfile = function (state) {
    return state.profile;
};

export const activeTabIndex = function(state) {
    return uiFlags.activeTabIndex(state.uiFlags);
};

export const projectAlreadyExists = function(state) {
    return uiFlags.projectAlreadyExists(state.uiFlags);
};

export const deleteProjectDlgVisible = function(state) {
    return uiFlags.deleteProjectDlgVisible(state.uiFlags);
};

export const checkedProjects = function(state) {
    return uiFlags.checkedProjects(state.uiFlags);
};