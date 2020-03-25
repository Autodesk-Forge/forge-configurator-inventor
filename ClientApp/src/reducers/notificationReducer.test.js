import {notificationReducer, initialState} from './notificationReducer';
import {addError, addLog} from '../actions/notificationActions';

describe('notification reducer', () => {
    test('should return the initial state', () => {
        expect(notificationReducer(undefined, {})).toEqual(initialState);
    })

    test('handles adding an error', () => {
        const newText = 'Some Error'
        const stateWithError = [
            '0 Errors',
            newText
        ]
        expect(notificationReducer(initialState, addError(newText))).toEqual(stateWithError);
    })

    test('handles adding a log', () => {
        const newText = 'A Log'
        const stateWithLog = [
            '0 Errors',
            newText
        ]
        expect(notificationReducer(initialState, addLog(newText))).toEqual(stateWithLog);
    })    
})