import {combineReducers} from 'redux';
import projectListReducer, * as list from './projectListReducers';
import {notificationReducer} from './notificationReducer';
import parametersReducer, * as params from './parametersReducer';
import updateParametersReducer, * as updateParams from './updateParametersReducer';
import uiFlagsReducer, * as uiFlasg from './uiFlagsReducer';

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer,
    parameters: parametersReducer,
    updateParameters: updateParametersReducer,
    uiFlagsReducer: uiFlagsReducer
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
    if (state.uiFlagsReducer.parametersEditedMessageClosed === true || state.uiFlagsReducer.parametersEditedMessageRejected === true )
        return false;
 
    const activeProject = getActiveProject(state);
 
    const parameters = getParameters(activeProject.id, state);
    const updateParameters = getUpdateParameters(activeProject.id, state);
 
    if (!parameters)
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
 
export const updateProgressShowing = function(state) {
    return uiFlasg.updateProgressShowing(state.uiFlagsReducer);
}

