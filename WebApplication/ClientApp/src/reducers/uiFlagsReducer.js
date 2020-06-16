import parametersActionTypes from '../actions/parametersActions';
import uiFlagsActionTypes from '../actions/uiFlagsActions';

export const initialState = {
   parametersEditedMessageClosed: false,
   parametersEditedMessageRejected: false,
   updateProgressShowing: false,
   rfaProgressShowing: null,
   rfaDownloadUrl: null,
   showUploadPackage: false,
   package: { file: '', root: ''}
};

export const updateProgressShowing = function(state) {
   return state.updateProgressShowing;
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
      case uiFlagsActionTypes.SHOW_UPDATE_PROGRESS:
         return { ...state, updateProgressShowing: action.visible};
      case uiFlagsActionTypes.SHOW_RFA_PROGRESS:
         return { ...state, rfaProgressShowing: action.visible, rfaDownloadUrl: null};
      case uiFlagsActionTypes.SET_RFA_LINK:
         return { ...state, rfaDownloadUrl: action.url};
      case uiFlagsActionTypes.SHOW_UPLOAD_PACKAGE:
         return { ...state, showUploadPackage: action.visible};
      case uiFlagsActionTypes.PACKAGE_FILE_EDITED:
         return { ...state, package: { file: action.file, root: state.package.root } };
      case uiFlagsActionTypes.PACKAGE_ROOT_EDITED:
         return { ...state, package: { file: state.package.file, root: action.file } };
      default:
         return state;
  }
}