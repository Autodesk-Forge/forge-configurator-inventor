/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import projectListReducer, * as list from './projectListReducers';
import { updateProjectList, updateActiveProject, updateProject, addProject } from '../actions/projectListActions';

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
            projects: list.sortProjects(newList)
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

        expect(projectListReducer(projectList, updateProject("1", { "svf": "newSvf" })).projects[0].svf).toEqual("newSvf");
    });

    it('correctly updates the downloadModelUrl', () => {
        const projectList = {
            activeProjectId: '1',
            projects: [{
                id : '1',
                svf: 'oldSvf'
                // no downloadModelUrl at all intentionally
            }]
        };

        expect(projectListReducer(projectList, updateProject("1", { "downloadModelUrl": "newUrl" })).projects[0].downloadModelUrl).toEqual("newUrl");
    });

    describe('addProject', () => {

        it('should work correctly with empty project list', () => {
            const newProject = {
                id: '3',
                label: 'Local Project 3',
                image: 'bike.png',
                svf: 'aaa111'
            };
            const result = projectListReducer(list.initialState, addProject(newProject));
            expect(result).toEqual({ activeProjectId: newProject.id, projects: [ newProject ] });
        });

        it('should preserve active project ID and insert project into correct place', () => {
            const existingProjects = [
                { id: '1', label: '1' },
                { id: '3', label: '3' }
            ];
            const initialState = {
                activeProjectId: '3', projects: existingProjects
            };

            // the project should be inserted in the middle of the existing projects
            const newProject = { id: '2', label: '2' };
            const result = projectListReducer(initialState, addProject(newProject));

            expect(result.activeProjectId).toEqual(initialState.activeProjectId); // active project is not changed
            expect(result.projects.map(p => p.id)).toEqual(['1', '2', '3']);
        });
    });
});