import {projectListReducer, initialState} from './projectListReducers';
import {updateProjectList} from '../actions/projectListActions';

describe('projectList reducer', () => {
    test('should return the initial state', () => {
        expect(projectListReducer(undefined, {})).toEqual(initialState);
    })

    test('handles empty project list', () => {
        expect(projectListReducer(initialState, updateProjectList([]))).toEqual([]);
    })

    it('handles new project list with label', () => {
        const newList = [
            {
                id: '7',
                label: 'New Project',
                image: 'new_image.png'
            }
        ]

        expect(projectListReducer(initialState, updateProjectList(newList))).toEqual(newList);
     })


     it('handles new project list with name', () => {
        const newList = [
            {
                id: '8',
                name: 'New Project B',
                image: 'new_image_B.png'
            }
        ]

        const newListWithLabel = [
            {
                id: '8',
                label: 'New Project B',
                image: 'new_image_B.png'
            }
        ]

        expect(projectListReducer(initialState, updateProjectList(newList))).toEqual(newListWithLabel);
     })     
})