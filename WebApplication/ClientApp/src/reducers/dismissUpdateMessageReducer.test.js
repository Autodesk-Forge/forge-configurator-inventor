import {dismissUpdateMessage} from '../actions/dismissUpdateMessageActions';
import dismissUpdateMessageReducer from './dismissUpdateMessageReducer';
import { editParameter, resetParameters } from '../actions/parametersActions';

describe('dismissUpdateMessage reducer', () => {
   it('check dismiss', () => {
      expect(dismissUpdateMessageReducer(undefined, dismissUpdateMessage())).toEqual(true);
      expect(dismissUpdateMessageReducer(undefined, editParameter("",""))).toEqual(false);
      expect(dismissUpdateMessageReducer(undefined, resetParameters(""))).toEqual(false);
   })
})
