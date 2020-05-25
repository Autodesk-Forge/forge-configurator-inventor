import {closeParametersEditedMessage, rejectParametersEditedMessage} from '../actions/uiFlagsActions';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import { editParameter, resetParameters } from '../actions/parametersActions';

export const stateParametersEditedMessageClosed = {...uiFlags.initialState, parametersEditedMessageClosed: true}; // not shown
export const stateParametersEditedMessageRejected = {...uiFlags.initialState, parametersEditedMessageRejected: true}; // not shown
export const stateParametersEditedMessageNotRejected = {...uiFlags.initialState, parametersEditedMessageRejected: false}; // shown

describe('uiFlags reducer', () => {
   it('check dismiss', () => {
      expect(uiFlagsReducer(uiFlags.initialState, closeParametersEditedMessage()).parametersEditedMessageClosed).toEqual(true);
      expect(uiFlagsReducer(stateParametersEditedMessageClosed, editParameter("","")).parametersEditedMessageClosed).toEqual(false);
      expect(uiFlagsReducer(stateParametersEditedMessageClosed, resetParameters("")).parametersEditedMessageClosed).toEqual(false);
   });
   it('By default is parameter edit message shown (not rejected)', () => {
      expect(uiFlagsReducer(undefined, {}).parametersEditedMessageRejected).toEqual(false);
   }),
   it('Sets from not rejected to rejected', () => {
      expect(uiFlagsReducer(stateParametersEditedMessageNotRejected, rejectParametersEditedMessage(false)).parametersEditedMessageRejected).toEqual(false);
   }),
   it('Sets from rejected to not rejected', () => {
      expect(uiFlagsReducer(stateParametersEditedMessageRejected, rejectParametersEditedMessage(true)).parametersEditedMessageRejected).toEqual(true);
   });
});
