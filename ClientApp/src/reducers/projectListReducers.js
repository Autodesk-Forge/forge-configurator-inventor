import projectListActionTypes from '../actions/projectListActions';

export const initialState = [
    {
        id: '1',
        label: 'Local Project 1',
        image: 'bike.png'
    },
    {
        id: '2',
        label: 'Another Local One',
        image: 'log.png'
    }
]

export const projectListReducer = function(state = initialState, action) {
    switch(action.type) {
        case projectListActionTypes.PROJECT_LIST_UPDATED: {
            // todo: project.label is required in toolbar for hig's project selector.
            // if we make the server to return correctly formatted project objects (with id, label, image), 
            // we can get rid of the map altogether and do just
            // return action.projectList;
            const projectList = action.projectList.map((project, index) => {
                return {
                    id: project.id,
                    label: project.name,
                    image: project.image
                }
            });
            return projectList;
        }
        default:
            return state;
    }
}