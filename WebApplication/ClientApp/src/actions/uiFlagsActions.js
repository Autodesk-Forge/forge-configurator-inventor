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

import repo from '../Repository';
import {addError} from './notificationActions';

export const actionTypes = {
    REJECT_PARAMETERS_EDITED_MESSAGE: 'REJECT_PARAMETERS_EDITED_MESSAGE',
    FETCH_SHOW_PARAMETERS_CHANGED: 'FETCH_SHOW_PARAMETERS_CHANGED',
    CLOSE_PARAMETERS_EDITED_MESSAGE: 'CLOSE_PARAMETERS_EDITED_MESSAGE',
    SHOW_MODAL_PROGRESS: 'SHOW_MODAL_PROGRESS',
    SHOW_UPDATE_FAILED: 'SHOW_UPDATE_FAILED',
    SHOW_LOGIN_FAILED: 'SHOW_LOGIN_FAILED',
    SHOW_DOWNLOAD_FAILED: 'SHOW_DOWNLOAD_FAILED',
    SET_ERROR_DATA: 'SET_ERROR_DATA',
    SHOW_DOWNLOAD_PROGRESS: 'SHOW_DOWNLOAD_PROGRESS',
    HIDE_DOWNLOAD_PROGRESS: 'HIDE_DOWNLOAD_PROGRESS',
    SET_DOWNLOAD_LINK: 'SET_DOWNLOAD_LINK',
    SHOW_UPLOAD_PACKAGE: 'SHOW_UPLOAD_PACKAGE',
    PACKAGE_FILE_EDITED: 'PACKAGE_FILE_EDITED',
    PACKAGE_ROOT_EDITED: 'PACKAGE_ROOT_EDITED',
    UPDATE_ACTIVE_TAB_INDEX: 'UPDATE_ACTIVE_TAB_INDEX',
    PROJECT_EXISTS: 'PROJECT_EXISTS',
    SHOW_DELETE_PROJECT: 'SHOW_DELETE_PROJECT',
    SET_PROJECT_CHECKED: 'SET_PROJECT_CHECKED',
    SET_CHECKED_PROJECTS: 'SET_CHECKED_PROJECTS',
    CLEAR_CHECKED_PROJECTS: 'CLEAR_CHECKED_PROJECTS',
    SHOW_DRAWING_PROGRESS: 'SHOW_DRAWING_PROGRESS',
    SHOW_ADOPT_WITH_PROPERTIES_PROGRESS: 'SHOW_ADOPT_WITH_PROPERTIES_PROGRESS',
    SHOW_ADOPT_WITH_PARAMS_FAILED: 'SHOW_ADOPT_WITH_PARAMS_FAILED',
    SET_DRAWING_URL: 'SET_DRAWING_URL',
    INVALIDATE_DRAWING: 'INVALIDATE_DRAWING',
    SET_STATS: 'SET_STATS',
    SET_REPORT_URL: 'SET_REPORT_URL',
    ACTIVE_DRAWING_UPDATED: 'ACTIVE_DRAWING_UPDATED',
    DRAWING_LIST_UPDATED: 'DRAWING_LIST_UPDATED'
};

export default actionTypes;

export const closeParametersEditedMessage = () => {
    return {
        type: actionTypes.CLOSE_PARAMETERS_EDITED_MESSAGE
    };
};

export const hideUpdateMessageBanner = (permanently) => async (dispatch) => {

    if (permanently === true) {
       const result = await repo.sendShowParametersChanged(false);
       dispatch(rejectParametersEditedMessage(!result));
    }

    dispatch(closeParametersEditedMessage());
};

export const rejectParametersEditedMessage = (show) => {
    return {
        type: actionTypes.REJECT_PARAMETERS_EDITED_MESSAGE,
        show
    };
};

export const fetchShowParametersChanged = () => async (dispatch) => {
    try {
        const showParametersChanged = await repo.loadShowParametersChanged();
        dispatch(rejectParametersEditedMessage(!showParametersChanged));
    } catch (error) {
        dispatch(addError('Failed to get information about "show changed parameters" . (' + error + ')'));
    }
};

export const showModalProgress = (visible) => {
    return {
        type: actionTypes.SHOW_MODAL_PROGRESS,
        visible
    };
};

export const showUpdateFailed = (visible) => {
    return {
        type: actionTypes.SHOW_UPDATE_FAILED,
        visible
    };
};

export const showLoginFailed = (visible) => {
    return {
        type: actionTypes.SHOW_LOGIN_FAILED,
        visible
    };
};

export const showDownloadFailed = (visible) => {
    return {
        type: actionTypes.SHOW_DOWNLOAD_FAILED,
        visible
    };
};

export const setErrorData = (errorData) => {
    return {
        type: actionTypes.SET_ERROR_DATA,
        errorData
    };
};

export const showDownloadProgress = (visible, title) => {
    return {
        type: actionTypes.SHOW_DOWNLOAD_PROGRESS,
        visible,
        title
    };
};

export const hideDownloadProgress = () => {
    return {
        type: actionTypes.HIDE_DOWNLOAD_PROGRESS
    };
};

export const showDrawingDownloadModalProgress = (visible) => {
    return {
        type: actionTypes.SHOW_DRAWING_DOWNLOAD_PROGRESS,
        visible
    };
};

export const setDownloadLink = (url) => {
    return {
        type: actionTypes.SET_DOWNLOAD_LINK,
        url
    };
};

export const showUploadPackage = (visible) => {
    return {
        type: actionTypes.SHOW_UPLOAD_PACKAGE,
        visible
    };
};

export const editPackageFile = (file, assemblies) => {
    return {
        type: actionTypes.PACKAGE_FILE_EDITED,
        file,
        assemblies
    };
};

export const editPackageRoot = (file) => {
    return {
        type: actionTypes.PACKAGE_ROOT_EDITED,
        file
    };
};

export const updateActiveTabIndex = (index) => {
    return {
        type: actionTypes.UPDATE_ACTIVE_TAB_INDEX,
        index
    };
};

export const setProjectAlreadyExists = (exists) => {
    return {
        type: actionTypes.PROJECT_EXISTS,
        exists
    };
};

export const showDeleteProject = (visible) => {
    return {
        type: actionTypes.SHOW_DELETE_PROJECT,
        visible
    };
};

export const setProjectChecked = (projectId, checked) => {
    return {
        type: actionTypes.SET_PROJECT_CHECKED,
        projectId,
        checked
    };
};

export const setCheckedProjects = (projects) => {
    return {
        type: actionTypes.SET_CHECKED_PROJECTS,
        projects
    };
};

export const clearCheckedProjects = () => {
    return {
        type: actionTypes.CLEAR_CHECKED_PROJECTS
    };
};

export const showDrawingExportProgress = (visible) => {
    return {
        type: actionTypes.SHOW_DRAWING_PROGRESS,
        visible
    };
};

export const showAdoptWithParametersProgress = (visible) => {
    return {
        type: actionTypes.SHOW_ADOPT_WITH_PROPERTIES_PROGRESS,
        visible
    };
};

export const showAdoptWithParamsFailed = (visible) => {
    return {
        type: actionTypes.SHOW_ADOPT_WITH_PARAMS_FAILED,
        visible
    };
};

export const setDrawingPdfUrl = (drawingKey, url) => {
    return {
        type: actionTypes.SET_DRAWING_URL,
        drawingKey,
        url
    };
};

export const invalidateDrawing = () => {
    return {
        type: actionTypes.INVALIDATE_DRAWING
    };
};

export const setStats = (stats, key) => {
    return {
        type: actionTypes.SET_STATS,
        stats,
        key
    };
};

export const setReportUrl = (reportUrl) => {
    return {
        type: actionTypes.SET_REPORT_URL,
        reportUrl
    };
};

export const updateActiveDrawing = activeDrawing => {
    return {
        type: actionTypes.ACTIVE_DRAWING_UPDATED,
        activeDrawing
    };
};

export const updateDrawingsList = drawingsList => {
    return {
        type: actionTypes.DRAWING_LIST_UPDATED,
        drawingsList
    };
};