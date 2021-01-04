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

import projectListActionTypes from '../actions/projectListActions';

export const initialState = {
    activeProjectId: null
};

export const getActiveProject = function(state) {
    // when no projects are available, returns empty project for correct UI initialization
    // TODO: (although it seems to be dangerous for other pieces of code, where we are just checking activeProject not to be null: !activeProject , or activeProject?.something)
    if (! state.projects || state.projects.length === 0) return { };
    return getProject(state.activeProjectId, state);
};

export const getProject = function(id, state) {
    if (! state.projects) return undefined;
    return state.projects.find(proj => proj.id === id);
};

export const getAdoptWarnings = function(id, state) {
    return getProject(id,state)?.adoptWarnings;
};

/** Generate shallow array clone, sorted by project label. */
export function sortProjects(projects) {
    return [].concat(projects).sort((a,b) => a.label.localeCompare(b.label));
}

/**
 * Decide on active project by using previously selected project ID and project list:
 * select the previous active project if present, or first project otherwise.
 *
 * @param projects Current list of projects. NOTE: it's expected the projects are sorted.
 * @param currId ID of current active project. Can be undefinded, or missed in the project list.
 */
function ensureActiveProjectId(projects, currId) {
    const activeProject = projects.find(({ id }) => id === currId);
    const prj = activeProject ? activeProject : (projects.length ? projects[0] : null);
    const prjId = prj ? prj.id : null;
    return prjId;
}

export default function(state = initialState, action) {
    switch(action.type) {
        case projectListActionTypes.PROJECT_LIST_UPDATED: {

            const sortedProjects = sortProjects(action.projectList);
            const prjId = ensureActiveProjectId(sortedProjects, state.activeProjectId);
            return { activeProjectId: prjId, projects: sortedProjects };
        }
        case projectListActionTypes.ACTIVE_PROJECT_UPDATED: {
            return { ...state, activeProjectId: action.activeProjectId};
        }
        case projectListActionTypes.UPDATE_PROJECT: {
            const projects = state.projects.map((project) => {
                return project.id !== action.activeProjectId ? project : {
                    ...project, ...action.data
                };
            });
            return { ...state, projects };
        }
        case projectListActionTypes.ADD_PROJECT: {
            const filteredList = state.projects ? state.projects.filter((project) => project.id !== action.newProject.id) : [];
            const updatedList = filteredList.concat(action.newProject);

            const sortedProjects = sortProjects(updatedList);
            const prjId = ensureActiveProjectId(sortedProjects, state.activeProjectId);
            return { activeProjectId: prjId, projects: sortedProjects };
        }
        default:
            return state;
    }
}