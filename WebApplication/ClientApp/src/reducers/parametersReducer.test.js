import { parametersReducer, initialState } from './parametersReducer';
import { updateParameters, editParameter, resetParameters } from '../actions/parametersActions';

describe('parameters reducer', () => {
    test('should return the initial state', () => {
        expect(parametersReducer(undefined, {})).toEqual(initialState);
    });

    test('handles update parameters for a project', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string"
            }
        ];

        const initialState = {};

        let expectedState = {};
        expectedState[projectId] = parameterSet;

        expect(parametersReducer(initialState, updateParameters(projectId, parameterSet))).toMatchObject(expectedState);
    });

    it('does nothing on edit', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string"
            }
        ];

        let initialState = {};
        initialState[projectId] = parameterSet;

        const newParameterValue = {
            name: "XYZ",
            value: "a new string"
        };

        expect(parametersReducer(initialState, editParameter(projectId, newParameterValue))).toMatchObject(initialState);
    });

    it('does nothing on reset', () => {
        const projectId = 'Conveyor';

        const parameterSet = [
            {
                name: "ABC",
                value: 123
            },
            {
                name: "XYZ",
                value: "a string"
            }
        ];

        let initialState = {};
        initialState[projectId] = parameterSet;

        expect(parametersReducer(initialState, resetParameters(projectId))).toMatchObject(initialState);
    });
});