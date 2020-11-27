/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import {combineReducers} from 'redux';
import projectListReducer, * as list from './projectListReducers';
import {notificationReducer} from './notificationReducer';
import parametersReducer, * as params from './parametersReducer';
import updateParametersReducer, * as updateParams from './updateParametersReducer';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import profileReducer from './profileReducer';
import bomReducer, * as bom from './bomReducer';
import { compareParameters } from "../actions/parametersActions";

export const mainReducer = combineReducers({
    projectList: projectListReducer,
    notifications: notificationReducer,
    parameters: parametersReducer,
    updateParameters: updateParametersReducer,
    uiFlags: uiFlagsReducer,
    profile: profileReducer,
    bom: bomReducer
});

export const getActiveProject = function(state) {
    return list.getActiveProject(state.projectList);
};

export const getProject = function(id, state) {
    return list.getProject(id, state.projectList);
};

export const getAdoptWarnings = function(projectId, state) {
    return list. getAdoptWarnings(projectId, state.projectList);
};

export const getParameters = function(projectId, state) {
    return params.getParameters(projectId, state.parameters);
};

export const getUpdateParameters = function(projectId, state) {
    return updateParams.getParameters(projectId, state.updateParameters);
};

export const parametersEditedMessageVisible = function(state) {
    const loggedIn = !state.profile.isLoggedIn;

    if (state.uiFlags.parametersEditedMessageClosed === true || (loggedIn && state.uiFlags.parametersEditedMessageRejected === true) )
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
        if (!compareParameters(parameter, updateParameter)) {
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

export const loginFailedShowing = function(state) {
    return uiFlags.loginFailedShowing(state.uiFlags);
};

export const downloadFailedShowing = function(state) {
    return uiFlags.downloadFailedShowing(state.uiFlags);
};

export const downloadDrawingFailedShowing = function(state) {
    return uiFlags.downloadDrawingFailedShowing(state.uiFlags);
};

export const errorData = function(state) {
    return uiFlags.errorData(state.uiFlags);
};

/** If downloads are generating, and "In progress" dialog is shown */
export const downloadProgressShowing = function(state) {
    return uiFlags.downloadProgressShowing(state.uiFlags);
};

export const downloadProgressTitle = function(state) {
    return uiFlags.downloadProgressTitle(state.uiFlags);
};

export const downloadUrl = function(state) {
    return uiFlags.downloadUrl(state.uiFlags);
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

export const uploadFailedShowing = function(state) {
    return uiFlags.uploadFailedShowing(state.uiFlags);
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

export const getBom = function(projectId, state) {
    return bom.getBom(projectId, state.bom);
};

export const getDrawingPdfUrl = function(state) {
    return uiFlags.getDrawingPdfUrl(state.uiFlags.activeDrawing, state.uiFlags);
};

export const drawingProgressShowing = function(state) {
    return uiFlags.drawingProgressShowing(state.uiFlags);
};

export const getStats = function(state) {
    return uiFlags.getStats(state.uiFlags);
};

export const getReportUrl = function(state) {
    return uiFlags.getReportUrl(state.uiFlags);
};

export const getDrawingsList = function(state) {
    return uiFlags.getDrawingsList(state.uiFlags);
};

export const getActiveDrawing = function(state) {
    return uiFlags.getActiveDrawing(state.uiFlags);
};

