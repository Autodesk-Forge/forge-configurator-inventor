import * as uiFlags from './uiFlagsReducer';
export const stateParametersEditedMessageClosed = {...uiFlags.initialState, parametersEditedMessageClosed: true}; // not shown
export const stateParametersEditedMessageRejected = {...uiFlags.initialState, parametersEditedMessageRejected: true}; // not shown
export const stateParametersEditedMessageNotRejected = {...uiFlags.initialState, parametersEditedMessageRejected: false}; // shown