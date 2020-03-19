export const PROJECT_LIST = 'PROJECT_LIST'

export const updateProjectList = projectList => {
    return {
        type: PROJECT_LIST,
        projectList
    }
}