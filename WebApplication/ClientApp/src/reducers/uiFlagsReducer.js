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

import parametersActionTypes from '../actions/parametersActions';
import uiFlagsActionTypes from '../actions/uiFlagsActions';
import uploadPackagesActionTypes from '../actions/uploadPackageActions';

export const initialState = {
   parametersEditedMessageClosed: false,
   parametersEditedMessageRejected: false,
   modalProgressShowing: false,
   updateFailedShowing: false,
   loginFailedShowing: false,
   downloadFailedShowing: false,
   reportUrl: null,
   downloadProgressShowing: null,
   downloadProgressTitle: null,
   downloadUrl: null,
   showUploadPackage: false,
   uploadProgressShowing: false,
   uploadProgressStatus: null,
   package: { file: null, root: '', assemblies: null },
   uploadFailedShowing: false,
   activeTabIndex: 0,
   projectAlreadyExists: false,
   showDeleteProject: false,
   checkedProjects: [],
   drawingProgressShowing: false,
   drawingUrl: null
};

export const modalProgressShowing = function(state) {
   return state.modalProgressShowing;
};

export const updateFailedShowing = function(state) {
   return state.updateFailedShowing;
};

export const loginFailedShowing = function(state) {
   return state.loginFailedShowing;
};

export const downloadFailedShowing = function(state) {
   return state.downloadFailedShowing;
};

export const downloadDrawingFailedShowing = function(state) {
   return state.downloadDrawingFailedShowing;
};

export const reportUrl = function(state) {
   return state.reportUrl;
};

export const downloadProgressShowing = function(state) {
   return state.downloadProgressShowing;
};

export const downloadProgressTitle = function(state) {
   return state.downloadProgressTitle;
};

export const downloadUrl = function(state) {
   return state.downloadUrl;
};

export const uploadPackageDlgVisible = function(state) {
   return state.showUploadPackage;
};

export const uploadPackageData = function(state) {
   return state.package;
};

export const uploadProgressShowing = function(state) {
   return state.uploadProgressShowing;
};

export const uploadProgressIsDone = function(state) {
   return state.uploadProgressStatus === "done";
};

export const uploadFailedShowing = function(state) {
   return state.uploadFailedShowing;
};

export const activeTabIndex = function(state) {
   return state.activeTabIndex;
};

export const projectAlreadyExists = function(state) {
   return state.projectAlreadyExists;
};

export const deleteProjectDlgVisible = function(state) {
   return state.showDeleteProject;
};

export const checkedProjects = function(state) {
   return state.checkedProjects;
};

export const getDrawingPdfUrl = function(state) {
   return state.drawingUrl;
};

export const drawingProgressShowing = function(state) {
   return state.drawingProgressShowing;
};

export default function(state = initialState, action) {
   switch(action.type) {
      case uiFlagsActionTypes.CLOSE_PARAMETERS_EDITED_MESSAGE:
          return { ...state, parametersEditedMessageClosed: true};
      case parametersActionTypes.PARAMETER_EDITED:
         return { ...state, parametersEditedMessageClosed: false};
      case parametersActionTypes.PARAMETERS_RESET:
         return { ...state, parametersEditedMessageClosed: false};
      case uiFlagsActionTypes.REJECT_PARAMETERS_EDITED_MESSAGE:
         return { ...state, parametersEditedMessageRejected: action.show };
      case uiFlagsActionTypes.SHOW_MODAL_PROGRESS:
         return { ...state, modalProgressShowing: action.visible};
      case uiFlagsActionTypes.SHOW_UPDATE_FAILED:
         return { ...state, updateFailedShowing: action.visible};
      case uiFlagsActionTypes.SHOW_LOGIN_FAILED:
         return { ...state, loginFailedShowing: action.visible};
      case uiFlagsActionTypes.SHOW_DOWNLOAD_FAILED:
         return { ...state, downloadFailedShowing: action.visible};
      case uiFlagsActionTypes.SET_REPORT_URL:
         return { ...state, reportUrl: action.url};
      case uiFlagsActionTypes.SHOW_DOWNLOAD_PROGRESS:
         return { ...state, downloadProgressShowing: action.visible, downloadUrl: null, downloadProgressTitle: action.title };
      case uiFlagsActionTypes.SET_DOWNLOAD_LINK:
         return { ...state, downloadUrl: action.url};
      case uiFlagsActionTypes.SHOW_UPLOAD_PACKAGE:
         return { ...state, showUploadPackage: action.visible};
      case uploadPackagesActionTypes.SET_UPLOAD_PROGRESS_VISIBLE:
         return { ...state, uploadProgressShowing: true};
      case uploadPackagesActionTypes.SET_UPLOAD_PROGRESS_HIDDEN:
         return { ...state, uploadProgressShowing: false, uploadProgressStatus: null};
      case uploadPackagesActionTypes.SET_UPLOAD_PROGRESS_DONE:
         return { ...state, uploadProgressStatus: "done"};
      case uploadPackagesActionTypes.SET_UPLOAD_FAILED:
         return { ...state, uploadFailedShowing: true, reportUrl: action.reportUrl };
      case uploadPackagesActionTypes.HIDE_UPLOAD_FAILED:
         return { ...state, uploadFailedShowing: false };
      case uiFlagsActionTypes.PACKAGE_FILE_EDITED:
         return { ...state, package: { file: action.file, assemblies: action.assemblies, root: '' } };
      case uiFlagsActionTypes.PACKAGE_ROOT_EDITED:
         return { ...state, package: { file: state.package.file, assemblies: state.package.assemblies, root: action.file } };
      case uiFlagsActionTypes.UPDATE_ACTIVE_TAB_INDEX:
         return { ...state, activeTabIndex: action.index};
      case uiFlagsActionTypes.PROJECT_EXISTS:
         return { ...state, projectAlreadyExists: action.exists};
      case uiFlagsActionTypes.SHOW_DELETE_PROJECT:
         return { ...state, showDeleteProject: action.visible};
      case uiFlagsActionTypes.SET_PROJECT_CHECKED:
         {
            const idx = state.checkedProjects.indexOf(action.projectId);
            let checkedProjects = [];
            if(action.checked) {
               // add projectId or nothing
               checkedProjects = state.checkedProjects.slice();
               if(idx === -1) {
                  checkedProjects = checkedProjects.concat([action.projectId]);
               }
            } else {
               // remove
               checkedProjects = state.checkedProjects.filter( id => id !== action.projectId);
            }
            return { ...state, checkedProjects };
         }
      case uiFlagsActionTypes.SET_CHECKED_PROJECTS:
         return { ...state, checkedProjects: action.projects};
      case uiFlagsActionTypes.CLEAR_CHECKED_PROJECTS:
         return { ...state, checkedProjects: []};
      case uiFlagsActionTypes.SHOW_DRAWING_DOWNLOAD_FAILED:
         return { ...state, downloadDrawingFailedShowing: action.visible};
      case uiFlagsActionTypes.SHOW_DRAWING_PROGRESS:
         return { ...state, drawingProgressShowing: action.visible};
      case uiFlagsActionTypes.SET_DRAWING_URL:
         return { ...state, drawingUrl: action.url };
      case uiFlagsActionTypes.INVALIDATE_DRAWING:
         return { ...state, drawingUrl: null };
      default:
         return state;
  }
}