import parametersActionTypes from '../actions/parametersActions';
import uiFlagsActionTypes from '../actions/uiFlagsActions';

export const initialState = {
   parametersEditedMessageClosed: false,
   parametersEditedMessageRejected: false,
   updateProgressShowing: false,
   rfaProgressProjectId: null
};

export const updateProgressShowing = function(state) {
   return state.updateProgressShowing;
};

export const rfaProgressProjectId = function(state) {
   return state.rfaProgressProjectId;
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
         return { ...state, rfaProgressProjectId: action.projectId};
      case uiFlagsActionTypes.HIDE_RFA_PROGRESS:
         return { ...state, rfaProgressProjectId: null};
            default:
         return state;
  }
}