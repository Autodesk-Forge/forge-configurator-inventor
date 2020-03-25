const actionTypes = {
    PROJECT_LIST_UPDATED: 'PROJECT_LIST_UPDATED',
    ACTIVE_PROJECT_UPDATED: 'ACTIVE_PROJECT_UPDATED'
}

export default actionTypes;

export const updateProjectList = projectList => {
    return {
        type: actionTypes.PROJECT_LIST_UPDATED,
        projectList
    }
}

export const updateActiveProject = activeProjectId => {
    return {
        type: actionTypes.ACTIVE_PROJECT_UPDATED,
        activeProjectId
    }
}