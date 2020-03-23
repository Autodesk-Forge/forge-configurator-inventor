const actionTypes = {
    PROJECT_LIST_UPDATED: 'PROJECT_LIST_UPDATED'
}

export default actionTypes;

export const updateProjectList = projectList => {
    return {
        type: actionTypes.PROJECT_LIST_UPDATED,
        projectList
    }
}