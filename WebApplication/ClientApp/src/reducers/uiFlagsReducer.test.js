import {closeParametersEditedMessage, rejectParametersEditedMessage} from '../actions/uiFlagsActions';
import uiFlagsReducer, * as uiFlags from './uiFlagsReducer';
import { editParameter, resetParameters } from '../actions/parametersActions';
import { stateParametersEditedMessageClosed, stateParametersEditedMessageNotRejected, stateParametersEditedMessageRejected } from './uiFlagsTestStates';


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
