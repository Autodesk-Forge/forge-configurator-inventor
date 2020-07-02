import parametersActionTypes from '../actions/parametersActions';
import uiFlagsActionTypes from '../actions/uiFlagsActions';
import uploadPackagesActionTypes from '../actions/uploadPackageActions';

export const initialState = {
   parametersEditedMessageClosed: false,
   parametersEditedMessageRejected: false,
   modalProgressShowing: false,
   updateFailedShowing: false,
   downloadRfaFailedShowing: false,
   reportUrl: null,
   rfaProgressShowing: null,
   rfaDownloadUrl: null,
   showUploadPackage: false,
   uploadProgressShowing: false,
   uploadProgressStatus: null,
   package: { file: '', root: ''},
   uploadFailedShowing: false,
   activeTabIndex: 0,
   projectAlreadyExists: false,
   showDeleteProject: false,
   checkedProjects: []
};

export const modalProgressShowing = function(state) {
   return state.modalProgressShowing;
};

export const updateFailedShowing = function(state) {
   return state.updateFailedShowing;
};

export const downloadRfaFailedShowing = function(state) {
   return state.downloadRfaFailedShowing;
};

export const reportUrl = function(state) {
   return state.reportUrl;
};

export const rfaProgressShowing = function(state) {
   return state.rfaProgressShowing;
};

export const rfaDownloadUrl = function(state) {
   return state.rfaDownloadUrl;
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
}

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
      case uiFlagsActionTypes.SHOW_RFA_FAILED:
         return { ...state, downloadRfaFailedShowing: action.visible};
      case uiFlagsActionTypes.SET_REPORT_URL:
         return { ...state, reportUrl: action.url};
      case uiFlagsActionTypes.SHOW_RFA_PROGRESS:
         return { ...state, rfaProgressShowing: action.visible, rfaDownloadUrl: null};
      case uiFlagsActionTypes.SET_RFA_LINK:
         return { ...state, rfaDownloadUrl: action.url};
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
      case uploadPackagesActionTypes.SHOW_UPLOAD_FAILED:
         return { ...state, uploadFailedShowing: action.visible };
      case uiFlagsActionTypes.PACKAGE_FILE_EDITED:
         return { ...state, package: { file: action.file, root: state.package.root } };
      case uiFlagsActionTypes.PACKAGE_ROOT_EDITED:
         return { ...state, package: { file: state.package.file, root: action.file } };
      case uiFlagsActionTypes.UPDATE_ACTIVE_TAB_INDEX:
         return { ...state, activeTabIndex: action.index};
      case uiFlagsActionTypes.PROJECT_EXISTS:
         return { ...state, projectAlreadyExists: action.exists};
      case uiFlagsActionTypes.SHOW_DELETE_PROJECT:
         return { ...state, showDeleteProject: action.visible};
      case uiFlagsActionTypes.SET_CHECKED_PROJECTS:
         return { ...state, checkedProjects: action.projects};
      case uiFlagsActionTypes.CLEAR_CHECKED_PROJECTS:
         return { ...state, checkedProjects: []};
      default:
         return state;
  }
}