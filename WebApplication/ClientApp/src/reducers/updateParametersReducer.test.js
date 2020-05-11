import { updateParametersReducer, initialState } from './updateParametersReducer';
import { updateParameters, editParameter, resetParameters } from '../actions/parametersActions';

describe('updateParameters reducer', () => {
    test('should return the initial state', () => {
        expect(updateParametersReducer(undefined, {})).toEqual(initialState);
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

        const returnedState = updateParametersReducer(initialState, updateParameters(projectId, parameterSet));
        expect(returnedState).toMatchObject(expectedState);
    });

    it('handles edit a single parameter', () => {
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

        const newState = {
            "Conveoyr": [ 
                {
                    name: "ABC",
                    value: 123
                },
                {
                    name: "XYZ",
                    value: "a new string"
                }                
             ]
        };

        expect(updateParametersReducer(initialState, editParameter(projectId, newParameterValue))).toMatchObject(newState);
    });

    it('does reset parameters', () => {
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

        const newParameterSet = [
            {
                name: "DEF",
                value: 456
            },
            {
                name: "XYZ",
                value: "a string reset"
            }
        ];

        let expectedState = {};
        expectedState[projectId] = newParameterSet;

        expect(updateParametersReducer(initialState, resetParameters(projectId, newParameterSet))).toMatchObject(expectedState);
    });
});