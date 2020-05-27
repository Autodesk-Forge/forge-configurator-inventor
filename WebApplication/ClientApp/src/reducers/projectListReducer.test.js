import projectListReducer, * as list from './projectListReducers';
import {updateProjectList, updateActiveProject, updateSvf} from '../actions/projectListActions';

describe('projectList reducer', () => {
    test('should return the initial state', () => {
        expect(projectListReducer(undefined, {})).toEqual(list.initialState);
    });

    test('handles empty project list', () => {
        expect(projectListReducer(list.initialState, updateProjectList([]))).toEqual({"activeProjectId": null, "projects": []});
    });

    it('handles new project list', () => {
        const projectList = [
            {
                id: '7',
                label: 'New Project',
                image: 'new_image.png',
                svf: 'aaa111'
            },
            {
                id: '5',
                label: 'New Project B',
                image: 'new_image_B.png',
                svf: 'bbb222'
            }
        ];

        const expectedResult = {
            activeProjectId: '7',
            projects: projectList
        };

        expect(projectListReducer(list.initialState, updateProjectList(projectList))).toEqual(expectedResult);
    });

    it('handles active project selection', () => {
        const secondProjectActive = { ...list.initialState, activeProjectId: '2'};

        expect(projectListReducer(list.initialState, updateActiveProject('2'))).toEqual(secondProjectActive);
    });

    it('keeps active project during project list update', () => {
        const secondProjectActive = { ...list.initialState, activeProjectId: '2' };

        // new data that include also the current active project
        const newList = [
            {
                id: '3',
                label: 'Local Project 3',
                image: 'bike.png',
                svf: 'aaa111'
            },
            {
                id: '2',
                label: 'Another Local One',
                image: 'logo.png',
                svf: 'bbb222'
            }
        ];

        const expectedResult = {
            activeProjectId: '2',
            projects: newList
        };

        expect(projectListReducer(secondProjectActive, updateProjectList(newList))).toEqual(expectedResult);
    });

    it('correctly selects active project', () => {
        const activeProject = {
            id: '2',
            svf: 'bbb222'
        };

        const firstProject = {
            id: '3',
            svf: 'aaa111'
        };

        const projects = [
            firstProject,
            activeProject
        ];

        const projectList = {
            activeProjectId: '2',
            projects: projects
        };

        expect(list.getActiveProject(projectList)).toEqual(activeProject);
    });

    it('correctly updates the svf', () => {
        const projectList = {
            activeProjectId: '1',
            projects: [{
                id : '1',
                svf: 'oldSvf'
            }]
        };

        expect(projectListReducer(projectList, updateSvf("1", "newSvf")).projects[0].svf).toEqual("newSvf");
    });
});