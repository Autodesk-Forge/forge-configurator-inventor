import { updateShowParametersChanged } from '../actions/applicationActions';
import showChangedParametersReducer from './showChangedParametersReducer';

describe('Show Changed Parameters reducer', () => {
    it('By default show changed parameters is true', () => {
        expect(showChangedParametersReducer(undefined, {})).toEqual(true);
    }),
    it('Sets from show to not show', () => {
        expect(showChangedParametersReducer(true, updateShowParametersChanged(false))).toEqual(false);
    }),
    it('Sets from not show to show', () => {
        expect(showChangedParametersReducer(false, updateShowParametersChanged(true))).toEqual(true);
    });
});
