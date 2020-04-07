import {projectListReducer, initialState} from './projectListReducers';
import {updateProjectList, updateActiveProject} from '../actions/projectListActions';

describe('projectList reducer', () => {
    test('should return the initial state', () => {
        expect(projectListReducer(undefined, {})).toEqual(initialState);
    })

    test('handles empty project list', () => {
        expect(projectListReducer(initialState, updateProjectList([]))).toEqual({"activeProjectId": null, "projects": []});
    })

    it('handles new project list', () => {
        const projectList = [
            {
                id: '7',
                label: 'New Project',
                image: 'new_image.png'
            },
            {
                id: '5',
                label: 'New Project B',
                image: 'new_image_B.png'
            }            
        ]

        const expectedResult = {
            activeProjectId: '7',
            projects: projectList
        }

        expect(projectListReducer(initialState, updateProjectList(projectList))).toEqual(expectedResult);
     })
     
     it('handles active project selection', () => {
        const secondProjectActive = { ...initialState, activeProjectId: '2'};

        expect(projectListReducer(initialState, updateActiveProject('2'))).toEqual(secondProjectActive);
     })

     it('keeps active project during project list update', () => {
         const secondProjectActive = { ...initialState, activeProjectId: '2' };

         // new data that include also the current active project
         const newList = [
            {
                id: '3',
                label: 'Local Project 3',
                image: 'bike.png'
            },
            {
                id: '2',
                label: 'Another Local One',
                image: 'logo.png'
            }
        ]

         const expectedResult = {
            activeProjectId: '2',
            projects: newList
        }

        expect(projectListReducer(secondProjectActive, updateProjectList(newList))).toEqual(expectedResult);
     })
})